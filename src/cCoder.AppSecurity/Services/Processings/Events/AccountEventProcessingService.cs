using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Processings.Events;

internal class AccountEventProcessingService(
    IAppService appService,
    IUserOrchestrationService userOrchestrationService,
    IRoleOrchestrationService roleOrchestrationService,
    IUserRoleOrchestrationService userRoleOrchestrationService) : IAccountEventProcessingService
{
    public async ValueTask ProcessAsync(SecurityAccountEvent accountEvent)
    {
        if (accountEvent?.User is null)
            return;

        App app = ResolveApp(accountEvent.RequestDomain);

        if (app is null)
            return;

        User user = await AddOrUpdateUserAsync(accountEvent, app);
        await AttachUsersRoleAsync(user, app.Id);
    }

    private App ResolveApp(string requestDomain)
    {
        if (string.IsNullOrWhiteSpace(requestDomain))
            return null;

        string normalizedDomain = NormalizeDomain(requestDomain);

        return appService.GetByDomain(normalizedDomain);
    }

    private async ValueTask<User> AddOrUpdateUserAsync(SecurityAccountEvent accountEvent, App app)
    {
        User user = userOrchestrationService.Get(accountEvent.User.Id);

        if (user is null)
        {
            user = new User
            {
                Id = accountEvent.User.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(accountEvent.Culture)
                    ? app.DefaultCultureId
                    : accountEvent.Culture,
                DisplayName = accountEvent.User.DisplayName,
                Email = accountEvent.User.Email,
                IsActive = !accountEvent.User.LockoutEnabled
            };

            return await userOrchestrationService.AddAsync(user);
        }

        user.DisplayName = accountEvent.User.DisplayName;
        user.Email = accountEvent.User.Email;
        user.IsActive = !accountEvent.User.LockoutEnabled;

        if (!string.IsNullOrWhiteSpace(accountEvent.Culture))
            user.DefaultCultureId = accountEvent.Culture;

        return await userOrchestrationService.UpdateAsync(user);
    }

    private async ValueTask AttachUsersRoleAsync(User user, int appId)
    {
        Role usersRole = roleOrchestrationService.GetAll(true)
            .FirstOrDefault(role => role.AppId == appId && role.Name == "Users");

        if (usersRole is null)
            return;

        bool roleAssigned = userRoleOrchestrationService.GetAll(true)
            .Any(userRole =>
                userRole.UserId == user.Id
                && userRole.RoleId == usersRole.Id);

        if (roleAssigned)
            return;

        await userRoleOrchestrationService.SaveAsync(
            new UserRole
            {
                UserId = user.Id,
                RoleId = usersRole.Id
            });
    }

    private static string NormalizeDomain(string requestDomain)
    {
        if (Uri.TryCreate(requestDomain, UriKind.Absolute, out Uri absoluteUri))
            return absoluteUri.Host;

        int portSeparatorIndex = requestDomain.IndexOf(':');

        return portSeparatorIndex < 0
            ? requestDomain
            : requestDomain[..portSeparatorIndex];
    }
}
