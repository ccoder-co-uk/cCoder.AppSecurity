using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data;
using EventLibrary.Models;
using DataRole = cCoder.Data.Models.Security.Role;
using DataUser = cCoder.Data.Models.Security.User;
using DataUserRole = cCoder.Data.Models.Security.UserRole;
using IUserEventBroker = cCoder.AppSecurity.Brokers.Events.IUserEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal class UserEventService(IUserEventBroker userEventBroker, ICoreAuthInfo authInfo)
    : IUserEventService
{
    public async ValueTask RaiseUserAddEventAsync(User entity)
    {
        EventMessage<DataUser> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalUser(entity),
        };

        await userEventBroker.RaiseUserAddEventAsync(message);
    }

    public async ValueTask RaiseUserUpdateEventAsync(User entity)
    {
        EventMessage<DataUser> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalUser(entity),
        };

        await userEventBroker.RaiseUserUpdateEventAsync(message);
    }

    public async ValueTask RaiseUserDeleteEventAsync(User entity)
    {
        EventMessage<DataUser> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalUser(entity),
        };

        await userEventBroker.RaiseUserDeleteEventAsync(message);
    }

    static DataUser ToExternalUser(User item) =>
        new()
        {
            Id = item.Id,
            DefaultCultureId = item.DefaultCultureId,
            DisplayName = item.DisplayName,
            Email = item.Email,
            IsActive = item.IsActive,
            DefaultCulture = item.DefaultCulture as cCoder.Data.Models.CMS.Culture,
            Roles = item.Roles?.Select(userRole => new DataUserRole
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
            }).ToArray(),
        };
}










