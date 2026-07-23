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

internal class PrivilegeService(
    IPrivilegeBroker privilegeBroker,
    IAuthorizationBroker authorizationBroker
) : IPrivilegeService
{
    public Privilege Get(string id)
    {
        Privilege privilege = GetAll().FirstOrDefault(predicate: i => i.Id == id);
        if (privilege is not null)
        {
            return privilege;
        }

        Privilege unrestrictedPrivilege = GetAll(ignoreFilters: true).FirstOrDefault(predicate: i => i.Id == id);
        if (unrestrictedPrivilege is not null)
        {
            throw new SecurityException(message: "Access Denied!");
        }

        return null;
    }

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        privilegeBroker.GetAllPrivileges(ignoreFilters: ignoreFilters)
            .Select(selector: ToLocalPrivilege)
            .AsQueryable();

    public async ValueTask<Privilege> AddAsync(Privilege privilege)
    {
        DataPrivilege internalPrivilege = new()
        {
            Id = privilege.Id,
            Type = privilege.Type,
            Operation = privilege.Operation,
            Description = privilege.Description,
            PortalAdminsOnly = privilege.PortalAdminsOnly
        };
        authorizationBroker.Authorize(
appId: privilegeBroker.GetAppId(entity: internalPrivilege),
privilege: $"{nameof(Privilege)}_create"
        );
        DataPrivilege result = await privilegeBroker.AddPrivilegeAsync(entity: internalPrivilege);
        privilege.Id = result.Id;
        privilege.Type = result.Type;
        privilege.Operation = result.Operation;
        privilege.Description = result.Description;
        privilege.PortalAdminsOnly = result.PortalAdminsOnly;
        return privilege;
    }

    public async ValueTask<Privilege> UpdateAsync(Privilege privilege)
    {
        DataPrivilege internalPrivilege = new()
        {
            Id = privilege.Id,
            Type = privilege.Type,
            Operation = privilege.Operation,
            Description = privilege.Description,
            PortalAdminsOnly = privilege.PortalAdminsOnly
        };
        authorizationBroker.Authorize(
appId: privilegeBroker.GetAppId(entity: internalPrivilege),
privilege: $"{nameof(Privilege)}_update"
        );
        DataPrivilege result = await privilegeBroker.UpdatePrivilegeAsync(entity: internalPrivilege);
        privilege.Id = result.Id;
        privilege.Type = result.Type;
        privilege.Operation = result.Operation;
        privilege.Description = result.Description;
        privilege.PortalAdminsOnly = result.PortalAdminsOnly;
        return privilege;
    }

    public async ValueTask DeleteAsync(string id)
    {
        Privilege privilege = Get(id: id);
        DataPrivilege internalPrivilege = ToExternalPrivilege(item: privilege);
        authorizationBroker.Authorize(
appId: privilegeBroker.GetAppId(entity: internalPrivilege),
privilege: $"{nameof(Privilege)}_delete"
        );
        _ = await privilegeBroker.DeletePrivilegeAsync(entity: internalPrivilege);
    }

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