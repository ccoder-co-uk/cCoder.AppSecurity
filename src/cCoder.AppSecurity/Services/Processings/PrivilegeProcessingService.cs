// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PrivilegeProcessingService(
    IPrivilegeService service,
    IAuthorizationBroker authorizationBroker
) : IPrivilegeProcessingService
{
    public Privilege Get(string privilegeId) =>
        TryCatch(operation: Privilege () =>
        {
            ValidateGet(
                privilegeId: privilegeId);

            authorizationBroker.Authorize(appId: null, privilege: "privilege_read");
            return service.Get(id: privilegeId);

        });

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Privilege> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            authorizationBroker.Authorize(appId: null, privilege: "privilege_read");
            return service.GetAll(ignoreFilters: ignoreFilters);

        });

    public ValueTask<Privilege> AddPrivilegeAsync(Privilege newPrivilege) =>
        TryCatch(operation: ValueTask<Privilege> () =>
        {
            ValidateAddPrivilege(
                newPrivilege: newPrivilege);

            authorizationBroker.Authorize(appId: null, privilege: "privilege_create");
            throw new InvalidOperationException(message: "Cannot add privileges");

        });

    public ValueTask<Privilege> UpdatePrivilegeAsync(Privilege updatedPrivilege) =>
        TryCatch(operation: ValueTask<Privilege> () =>
        {
            ValidateUpdatePrivilege(
                updatedPrivilege: updatedPrivilege);

            authorizationBroker.Authorize(appId: null, privilege: "privilege_update");
            throw new InvalidOperationException(message: "Cannot update privileges");

        });

    public ValueTask DeleteAsync(string privilegeId) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDelete(
                privilegeId: privilegeId);

            authorizationBroker.Authorize(appId: null, privilege: "privilege_delete");
            throw new InvalidOperationException(message: "Cannot delete privileges");

        });

    public ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdatePrivilege(
        IEnumerable<Privilege> items
    ) =>
        TryCatch(operation: async ValueTask<IEnumerable<Result<Privilege>>> () =>
        {
            ValidateAddOrUpdatePrivilege(
                items: items);

            List<Result<Privilege>> results = [];

            foreach (Privilege item in items)
            {
                try
                {
                    bool isAdd = string.IsNullOrWhiteSpace(value: item.Id);

                    results.Add(
    item: new Result<Privilege>
    {
        Success = true,
        Item = isAdd ? await AddPrivilegeValueAsync(newPrivilege: item) : await UpdatePrivilegeValueAsync(updatedPrivilege: item),
        Message = isAdd ? "Added Successfully" : "Updated Successfully",
    }
                    );
                }
                catch (Exception ex)
                {
                    results.Add(
    item: new Result<Privilege>
    {
        Success = false,
        Item = item,
        Message = ex.Message,
    }
                    );
                }
            }

            return results;

        });

    public ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> deletedPrivilege) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteAllPrivilege(
                deletedPrivilege: deletedPrivilege);

            foreach (Privilege item in deletedPrivilege)
            {
                await DeletePrivilegeValueAsync(privilegeId: item.Id);
            }

        });

    private ValueTask<Privilege> AddPrivilegeValueAsync(Privilege newPrivilege) =>
        AddPrivilegeAsync(newPrivilege: newPrivilege);

    private ValueTask<Privilege> UpdatePrivilegeValueAsync(Privilege updatedPrivilege) =>
        UpdatePrivilegeAsync(updatedPrivilege: updatedPrivilege);

    private ValueTask DeletePrivilegeValueAsync(string privilegeId) =>
        DeleteAsync(privilegeId: privilegeId);
}