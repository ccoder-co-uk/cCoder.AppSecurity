// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IUserRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IUserRoleEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class UserRoleEventService(IUserRoleEventBroker userRoleEventBroker, ICoreAuthInfo authInfo)
    : IUserRoleEventService
{
    public ValueTask RaiseUserRoleAddEventAsync(UserRole entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaiseUserRoleAddEvent(
                entity: entity);

            EventMessage<DataUserRole> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = ToExternalUserRole(item: entity),
            };

            await userRoleEventBroker.RaiseUserRoleAddEventAsync(message: message);

        });

    public ValueTask RaiseUserRoleDeleteEventAsync(UserRole entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaiseUserRoleDeleteEvent(
                entity: entity);

            EventMessage<DataUserRole> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = ToExternalUserRole(item: entity),
            };

            await userRoleEventBroker.RaiseUserRoleDeleteEventAsync(message: message);

        });

    static DataUserRole ToExternalUserRole(UserRole item) =>
        new()
        {
            RoleId = item.RoleId,
            UserId = item.UserId,
            User = item.User == null ? null : new DataUser
            {
                Id = item.User.Id,
                DefaultCultureId = item.User.DefaultCultureId,
                DisplayName = item.User.DisplayName,
                Email = item.User.Email,
                IsActive = item.User.IsActive,
                DefaultCulture = item.User.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            },
            Role = item.Role == null ? null : new DataRole
            {
                Id = item.Role.Id,
                AppId = item.Role.AppId,
                Name = item.Role.Name,
                Description = item.Role.Description,
                Privs = item.Role.Privs,
            },
        };
}