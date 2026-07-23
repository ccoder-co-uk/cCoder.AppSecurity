// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AccountEventOrchestrationService(
    IAppProcessingService appProcessingService,
    IUserProcessingService userProcessingService,
    IRoleProcessingService roleProcessingService,
    IUserRoleProcessingService userRoleProcessingService) : IAccountEventOrchestrationService
{
    public ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent accountEvent) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateProcessSecurityAccountEvent(
                accountEvent: accountEvent);

            if (accountEvent?.User is null)
            {
                return;
            }

            App app = ResolveApp(requestDomain: accountEvent.RequestDomain);

            if (app is null)
            {
                return;
            }

            User user = await AddOrUpdateUserAsync(accountEvent: accountEvent, app: app);
            await AttachUsersRoleAsync(user: user, appId: app.Id);

        });

    private App ResolveApp(string requestDomain)
    {
        if (string.IsNullOrWhiteSpace(value: requestDomain))
        {
            return null;
        }

        string normalizedDomain = NormalizeDomain(requestDomain: requestDomain);

        return appProcessingService.GetByDomain(domain: normalizedDomain);
    }

    private async ValueTask<User> AddOrUpdateUserAsync(SecurityAccountEvent accountEvent, App app)
    {
        User user = userProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: user =>
                user.Id == accountEvent.User.Id
                || user.Email == accountEvent.User.Email);

        if (user is null)
        {
            user = new User
            {
                Id = accountEvent.User.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(value: accountEvent.Culture)
                    ? app.DefaultCultureId
                    : accountEvent.Culture,
                DisplayName = accountEvent.User.DisplayName,
                Email = accountEvent.User.Email,
                IsActive = !accountEvent.User.LockoutEnabled
            };

            return await userProcessingService.AddUserAsync(entity: user);
        }

        user.DisplayName = accountEvent.User.DisplayName;
        user.Email = accountEvent.User.Email;
        user.IsActive = !accountEvent.User.LockoutEnabled;

        if (!string.IsNullOrWhiteSpace(value: accountEvent.Culture))
        {
            user.DefaultCultureId = accountEvent.Culture;
        }

        return await userProcessingService.UpdateUserAsync(entity: user);
    }

    private async ValueTask AttachUsersRoleAsync(User user, int appId)
    {
        Role usersRole = roleProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: role => role.AppId == appId && role.Name == "Users");

        if (usersRole is null)
        {
            return;
        }

        bool roleAssigned = userRoleProcessingService.GetAll(ignoreFilters: true)
            .Any(predicate: userRole =>
                userRole.UserId == user.Id
                && userRole.RoleId == usersRole.Id);

        if (roleAssigned)
        {
            return;
        }

        await userRoleProcessingService.SaveUserRoleAsync(
entity: new UserRole
{
    UserId = user.Id,
    RoleId = usersRole.Id
});
    }

    private static string NormalizeDomain(string requestDomain)
    {
        if (Uri.TryCreate(uriString: requestDomain, uriKind: UriKind.Absolute, result: out Uri absoluteUri))
        {
            return absoluteUri.Host;
        }

        int portSeparatorIndex = requestDomain.IndexOf(value: ':');

        return portSeparatorIndex < 0
            ? requestDomain
            : requestDomain[..portSeparatorIndex];
    }
}