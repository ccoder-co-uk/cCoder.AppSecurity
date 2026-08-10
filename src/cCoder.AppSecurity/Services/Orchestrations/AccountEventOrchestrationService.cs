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
                if (!appProcessingService
                    .GetAll()
                    .Any())
                {
                    await EnsureGuestUserAsync();

                    await AddOrUpdateUserAsync(
                        accountEvent: accountEvent,
                        app: null);
                }

                return;
            }

            User user = await AddOrUpdateUserAsync(accountEvent: accountEvent, app: app);

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
                    ? app?.DefaultCultureId ?? string.Empty
                    : accountEvent.Culture,
                DisplayName = accountEvent.User.DisplayName,
                Email = accountEvent.User.Email,
                IsActive = app is null || !accountEvent.User.LockoutEnabled
            };

            return await userProcessingService
                .AddUserFromAccountEventAsync(entity: user);
        }

        if (app is null)
        {
            return user;
        }

        user.DisplayName = accountEvent.User.DisplayName;
        user.Email = accountEvent.User.Email;
        user.IsActive = !accountEvent.User.LockoutEnabled;

        if (!string.IsNullOrWhiteSpace(value: accountEvent.Culture))
        {
            user.DefaultCultureId = accountEvent.Culture;
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