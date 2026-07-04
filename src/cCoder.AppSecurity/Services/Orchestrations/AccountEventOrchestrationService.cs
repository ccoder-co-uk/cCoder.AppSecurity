using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal class AccountEventOrchestrationService(
    IAppService appService,
    IUserProcessingService userProcessingService,
    IRoleProcessingService roleProcessingService,
    IUserRoleProcessingService userRoleProcessingService) : IAccountEventOrchestrationService
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
        User user = userProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(user =>
                user.Id == accountEvent.User.Id
                || user.Email == accountEvent.User.Email);

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

            return await userProcessingService.AddAsync(user);
        }

        user.DisplayName = accountEvent.User.DisplayName;
        user.Email = accountEvent.User.Email;
        user.IsActive = !accountEvent.User.LockoutEnabled;

        if (!string.IsNullOrWhiteSpace(accountEvent.Culture))
            user.DefaultCultureId = accountEvent.Culture;

        return await userProcessingService.UpdateAsync(user);
    }

    private async ValueTask AttachUsersRoleAsync(User user, int appId)
    {
        Role usersRole = roleProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(role => role.AppId == appId && role.Name == "Users");

        if (usersRole is null)
            return;

        bool roleAssigned = userRoleProcessingService.GetAll(ignoreFilters: true)
            .Any(userRole =>
                userRole.UserId == user.Id
                && userRole.RoleId == usersRole.Id);

        if (roleAssigned)
            return;

        await userRoleProcessingService.AddAsync(
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
