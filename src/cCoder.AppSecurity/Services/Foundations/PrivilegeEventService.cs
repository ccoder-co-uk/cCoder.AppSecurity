// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Data;
using cCoder.Eventing.Models;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;
using IPrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.IPrivilegeEventBroker;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class PrivilegeEventService(
    IPrivilegeEventBroker privilegeEventBroker,
    ICoreAuthInfo authInfo
) : IPrivilegeEventService
{
    public ValueTask RaisePrivilegeAddEventAsync(Privilege entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaisePrivilegeAddEvent(
                entity: entity);

            EventMessage<DataPrivilege> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = ToExternalPrivilege(item: entity),
            };

            await privilegeEventBroker.RaisePrivilegeAddEventAsync(message: message);

        });

    public ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaisePrivilegeUpdateEvent(
                entity: entity);

            EventMessage<DataPrivilege> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = ToExternalPrivilege(item: entity),
            };

            await privilegeEventBroker.RaisePrivilegeUpdateEventAsync(message: message);

        });

    public ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateRaisePrivilegeDeleteEvent(
                entity: entity);

            EventMessage<DataPrivilege> message = new()
            {
                AuthInfo = new EventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = ToExternalPrivilege(item: entity),
            };

            await privilegeEventBroker.RaisePrivilegeDeleteEventAsync(message: message);

        });

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