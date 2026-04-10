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
        Privilege privilege = GetAll().FirstOrDefault(i => i.Id == id);
        if (privilege is not null)
            return privilege;

        Privilege unrestrictedPrivilege = GetAll(true).FirstOrDefault(i => i.Id == id);
        if (unrestrictedPrivilege is not null)
            throw new SecurityException("Access Denied!");

        return null;
    }

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        privilegeBroker.GetAllPrivileges(ignoreFilters)
            .Select(ToLocalPrivilege)
            .AsQueryable();

    public async ValueTask<Privilege> AddAsync(Privilege privilege)
    {
        DataPrivilege internalPrivilege = ToExternalPrivilege(privilege);
        authorizationBroker.Authorize(
            privilegeBroker.GetAppId(internalPrivilege),
            $"{nameof(Privilege)}_create"
        );
        return ToLocalPrivilege(await privilegeBroker.AddPrivilegeAsync(internalPrivilege));
    }

    public async ValueTask<Privilege> UpdateAsync(Privilege privilege)
    {
        DataPrivilege internalPrivilege = ToExternalPrivilege(privilege);
        authorizationBroker.Authorize(
            privilegeBroker.GetAppId(internalPrivilege),
            $"{nameof(Privilege)}_update"
        );
        return ToLocalPrivilege(await privilegeBroker.UpdatePrivilegeAsync(internalPrivilege));
    }

    public async ValueTask DeleteAsync(string id)
    {
        Privilege privilege = Get(id);
        DataPrivilege internalPrivilege = ToExternalPrivilege(privilege);
        authorizationBroker.Authorize(
            privilegeBroker.GetAppId(internalPrivilege),
            $"{nameof(Privilege)}_delete"
        );
        _ = await privilegeBroker.DeletePrivilegeAsync(internalPrivilege);
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













