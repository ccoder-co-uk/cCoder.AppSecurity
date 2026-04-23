using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;
using IPrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.IPrivilegeEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal class PrivilegeEventService(
    IPrivilegeEventBroker privilegeEventBroker,
    ICoreAuthInfo authInfo
) : IPrivilegeEventService
{
    public async ValueTask RaisePrivilegeAddEventAsync(Privilege entity)
    {
        EventMessage<DataPrivilege> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalPrivilege(entity),
        };

        await privilegeEventBroker.RaisePrivilegeAddEventAsync(message);
    }

    public async ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity)
    {
        EventMessage<DataPrivilege> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalPrivilege(entity),
        };

        await privilegeEventBroker.RaisePrivilegeUpdateEventAsync(message);
    }

    public async ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity)
    {
        EventMessage<DataPrivilege> message = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = ToExternalPrivilege(entity),
        };

        await privilegeEventBroker.RaisePrivilegeDeleteEventAsync(message);
    }

    static DataPrivilege ToExternalPrivilege(Privilege item) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            Operation = item.Operation,
            Description = item.Description,
            PortalAdminsOnly = item.PortalAdminsOnly,
        };
}










