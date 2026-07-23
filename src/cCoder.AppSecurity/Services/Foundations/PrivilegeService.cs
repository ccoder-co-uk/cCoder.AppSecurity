// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using DataPrivilege = cCoder.Data.Models.Security.Privilege;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IPrivilegeBroker = cCoder.AppSecurity.Brokers.IPrivilegeBroker;


namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class PrivilegeService(
    IPrivilegeBroker privilegeBroker,
    IAuthorizationBroker authorizationBroker
) : IPrivilegeService
{
    public Privilege Get(string privilegeId) =>
        TryCatch(operation: Privilege () =>
        {
            ValidateGet(
                privilegeId: privilegeId);

            Privilege privilege = GetAll()
                .FirstOrDefault(predicate: i => i.Id == privilegeId);

            if (privilege is not null)
            {
                return privilege;
            }

            Privilege unrestrictedPrivilege = GetAll(ignoreFilters: true)
                .FirstOrDefault(predicate: i => i.Id == privilegeId);

            if (unrestrictedPrivilege is not null)
            {
                throw new SecurityException(message: "Access Denied!");
            }

            return null;

        });

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Privilege> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return privilegeBroker.GetAllPrivileges(ignoreFilters: ignoreFilters)
            .Select(selector: ToLocalPrivilege)
            .AsQueryable();
        });

    public ValueTask<Privilege> AddPrivilegeAsync(Privilege newPrivilege) =>
        TryCatch(operation: async ValueTask<Privilege> () =>
        {
            ValidateAddPrivilege(
                newPrivilege: newPrivilege);

            DataPrivilege internalPrivilege = new()
            {
                Id = newPrivilege.Id,
                Type = newPrivilege.Type,
                Operation = newPrivilege.Operation,
                Description = newPrivilege.Description,
                PortalAdminsOnly = newPrivilege.PortalAdminsOnly
            };

            authorizationBroker.Authorize(
    appId: privilegeBroker.GetAppId(entity: internalPrivilege),
    privilege: $"{nameof(Privilege)}_create"
            );

            DataPrivilege result = await privilegeBroker.AddPrivilegeAsync(entity: internalPrivilege);
            newPrivilege.Id = result.Id;
            newPrivilege.Type = result.Type;
            newPrivilege.Operation = result.Operation;
            newPrivilege.Description = result.Description;
            newPrivilege.PortalAdminsOnly = result.PortalAdminsOnly;
            return newPrivilege;

        });

    public ValueTask<Privilege> UpdatePrivilegeAsync(Privilege updatedPrivilege) =>
        TryCatch(operation: async ValueTask<Privilege> () =>
        {
            ValidateUpdatePrivilege(
                updatedPrivilege: updatedPrivilege);

            DataPrivilege internalPrivilege = new()
            {
                Id = updatedPrivilege.Id,
                Type = updatedPrivilege.Type,
                Operation = updatedPrivilege.Operation,
                Description = updatedPrivilege.Description,
                PortalAdminsOnly = updatedPrivilege.PortalAdminsOnly
            };

            authorizationBroker.Authorize(
    appId: privilegeBroker.GetAppId(entity: internalPrivilege),
    privilege: $"{nameof(Privilege)}_update"
            );

            DataPrivilege result = await privilegeBroker.UpdatePrivilegeAsync(entity: internalPrivilege);
            updatedPrivilege.Id = result.Id;
            updatedPrivilege.Type = result.Type;
            updatedPrivilege.Operation = result.Operation;
            updatedPrivilege.Description = result.Description;
            updatedPrivilege.PortalAdminsOnly = result.PortalAdminsOnly;
            return updatedPrivilege;

        });

    public ValueTask DeleteAsync(string privilegeId) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDelete(
                privilegeId: privilegeId);

            Privilege privilege = Get(privilegeId: privilegeId);
            DataPrivilege internalPrivilege = ToExternalPrivilege(item: privilege);

            authorizationBroker.Authorize(
    appId: privilegeBroker.GetAppId(entity: internalPrivilege),
    privilege: $"{nameof(Privilege)}_delete"
            );

            _ = await privilegeBroker.DeletePrivilegeAsync(entity: internalPrivilege);

        });

    static Privilege ToLocalPrivilege(DataPrivilege item) =>
        new()
        {
            Id = item.Id,
            Type = item.Type,
            Operation = item.Operation,
            Description = item.Description,
            PortalAdminsOnly = item.PortalAdminsOnly,
        };

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