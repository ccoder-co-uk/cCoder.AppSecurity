// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Models.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AccountEventOrchestrationService(
    IAppProcessingService appProcessingService,
    IUserProcessingService userProcessingService,
    IAccountRoleAssignmentProcessingService accountRoleAssignmentProcessingService)
    : IAccountEventOrchestrationService
{
    public ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent securityAccountEvent) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateProcessSecurityAccountEvent(
                securityAccountEvent: securityAccountEvent);

            if (securityAccountEvent?.User is null)
            {
                return;
            }

            App app = ResolveApp(requestDomain: securityAccountEvent.RequestDomain);

            if (app is null)
            {
                if (!appProcessingService
                    .GetAll()
                    .Any())
                {
                    await EnsureGuestUserAsync();

                    await AddOrUpdateUserAsync(
                        securityAccountEvent: securityAccountEvent,
                        app: null);
                }

                return;
            }

            User user = await AddOrUpdateUserAsync(securityAccountEvent: securityAccountEvent, app: app);

            await accountRoleAssignmentProcessingService.AttachUsersRoleAsync(
                user: user,
                appId: app.Id);

        });

    private async ValueTask EnsureGuestUserAsync()
    {
        bool guestExists = userProcessingService
            .GetAll(ignoreFilters: true)
            .Any(predicate: user => user.Id == "Guest");

        if (!guestExists)
        {
            try
            {
                await userProcessingService.AddUserAsync(entity: new User
                {
                    Id = "Guest",
                    DefaultCultureId = string.Empty,
                    DisplayName = "Guest",
                    Email = string.Empty,
                    IsActive = true
                });
            }
            catch (AppSecurityProcessingServiceException)
            {
                bool guestWasCreatedByAnotherProcess = userProcessingService
                    .GetAll(ignoreFilters: true)
                    .Any(predicate: user => user.Id == "Guest");

                if (!guestWasCreatedByAnotherProcess)
                {
                    throw;
                }
            }
        }
    }

    private App ResolveApp(string requestDomain)
    {
        if (string.IsNullOrWhiteSpace(value: requestDomain))
        {
            return null;
        }

        string normalizedDomain = NormalizeDomain(requestDomain: requestDomain);

        return appProcessingService.GetByDomain(domain: normalizedDomain);
    }

    private async ValueTask<User> AddOrUpdateUserAsync(SecurityAccountEvent securityAccountEvent, App app)
    {
        User user = userProcessingService.GetAll(ignoreFilters: true)
            .FirstOrDefault(predicate: user =>
                user.Id == securityAccountEvent.User.Id
                || user.Email == securityAccountEvent.User.Email);

        if (user is null)
        {
            user = new User
            {
                Id = securityAccountEvent.User.Id,
                DefaultCultureId = string.IsNullOrWhiteSpace(value: securityAccountEvent.Culture)
                    ? app?.DefaultCultureId ?? string.Empty
                    : securityAccountEvent.Culture,
                DisplayName = securityAccountEvent.User.DisplayName,
                Email = securityAccountEvent.User.Email,
                IsActive = app is null || !securityAccountEvent.User.LockoutEnabled
            };

            return await userProcessingService
                .AddUserFromAccountEventAsync(entity: user);
        }

        if (app is null)
        {
            return user;
        }

        user.DisplayName = securityAccountEvent.User.DisplayName;
        user.Email = securityAccountEvent.User.Email;
        user.IsActive = !securityAccountEvent.User.LockoutEnabled;

        if (!string.IsNullOrWhiteSpace(value: securityAccountEvent.Culture))
        {
            user.DefaultCultureId = securityAccountEvent.Culture;
        }

        return await userProcessingService
            .UpdateUserFromAccountEventAsync(entity: user);
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