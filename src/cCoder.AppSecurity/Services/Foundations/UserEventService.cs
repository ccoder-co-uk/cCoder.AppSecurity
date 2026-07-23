// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Eventing.Models;
using cCoder.AppSecurity.Brokers;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IUserEventBroker = cCoder.AppSecurity.Brokers.Events.IUserEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class UserEventService(IUserEventBroker userEventBroker, IAuthInfoBroker authInfoBroker)
    : IUserEventService
{
    public ValueTask RaiseUserAddEventAsync(User entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaiseUserAddEvent(
                entity: entity);

            EventMessage<DataUser> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSSOUserId() },
                Data = ToExternalUser(item: entity),
            };

            await userEventBroker.RaiseUserAddEventAsync(message: message);

        });

    public ValueTask RaiseUserUpdateEventAsync(User entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaiseUserUpdateEvent(
                entity: entity);

            EventMessage<DataUser> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSSOUserId() },
                Data = ToExternalUser(item: entity),
            };

            await userEventBroker.RaiseUserUpdateEventAsync(message: message);

        });

    public ValueTask RaiseUserDeleteEventAsync(User entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaiseUserDeleteEvent(
                entity: entity);

            EventMessage<DataUser> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfoBroker.GetSSOUserId() },
                Data = ToExternalUser(item: entity),
            };

            await userEventBroker.RaiseUserDeleteEventAsync(message: message);

        });

    static DataUser ToExternalUser(User item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            Roles = item.Roles?.Select(selector: userRole => new DataUserRole
            {
                RoleId = userRole.RoleId,
                UserId = userRole.UserId,
                Role = userRole.Role == null ? null : new DataRole
                {
                    Id = userRole.Role.Id,
                    AppId = userRole.Role.AppId,
                    Name = userRole.Role.Name,
                    Description = userRole.Role.Description,
                    Privs = userRole.Role.Privs,
                },
            })
                .ToArray(),
        };
}