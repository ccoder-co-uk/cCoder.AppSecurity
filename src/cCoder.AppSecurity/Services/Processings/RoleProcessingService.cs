// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;


namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class RoleProcessingService(IRoleService service) : IRoleProcessingService
{
    public Role Get(Guid roleId) =>
        TryCatch(operation: Role () =>
        {
            ValidateGet(
                roleId: roleId);

            return service.Get(id: roleId);
        });

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        TryCatch(operation: IQueryable<Role> () =>
        {
            ValidateGetAll(
                ignoreFilters: ignoreFilters);

            return service.GetAll(ignoreFilters: ignoreFilters);
        });

    public ValueTask<Role> AddRoleAsync(Role newRole) =>
        TryCatch(operation: ValueTask<Role> () =>
        {
            ValidateAddRole(
                newRole: newRole);

            return service.AddRoleAsync(role: newRole);
        });

    public ValueTask<Role> AddValidatedRoleAsync(Role newRole) =>
        TryCatch(operation: ValueTask<Role> () =>
        {
            ValidateAddValidatedRole(
                newRole: newRole);

            return service.AddValidatedRoleAsync(role: newRole);
        });

    public ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        TryCatch(operation: ValueTask<Role> () =>
        {
            ValidateUpdateRole(
                updatedRole: updatedRole);

            return service.UpdateRoleAsync(role: updatedRole);
        });

    public ValueTask<Role> UpdateValidatedRoleAsync(Role updatedRole) =>
        TryCatch(operation: ValueTask<Role> () =>
        {
            ValidateUpdateValidatedRole(
                updatedRole: updatedRole);

            return service.UpdateValidatedRoleAsync(role: updatedRole);
        });

    public ValueTask DeleteAsync(Guid roleId) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDelete(
                roleId: roleId);

            return service.DeleteAsync(id: roleId);
        });

    public ValueTask DeleteValidatedAsync(Guid roleId) =>
        TryCatch(operation: ValueTask () =>
        {
            ValidateDeleteValidated(
                roleId: roleId);

            return service.DeleteValidatedAsync(id: roleId);
        });

    public ValueTask<IEnumerable<Result<Role>>> AddOrUpdateRole(
        IEnumerable<Role> items
    ) =>
        TryCatch(operation: async ValueTask<IEnumerable<Result<Role>>> () =>
        {
            ValidateAddOrUpdateRole(
                items: items);

            List<Result<Role>> results = [];

            foreach (Role item in items)
            {
                try
                {
                    bool isAdd = item.Id == Guid.Empty;

                    results.Add(
    item: new Result<Role>
    {
        Success = true,
        Item = isAdd ? await AddRoleValueAsync(newRole: item) : await UpdateRoleValueAsync(updatedRole: item),
        Message = isAdd ? "Added Successfully" : "Updated Successfully",
    }
                    );
                }
                catch (Exception ex)
                {
                    results.Add(
    item: new Result<Role>
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

    public ValueTask DeleteAllRoleAsync(IEnumerable<Role> deletedRole) =>
        TryCatch(operation: async ValueTask () =>
        {
            ValidateDeleteAllRole(
                deletedRole: deletedRole);

            foreach (Role item in deletedRole)
            {
                await DeleteRoleValueAsync(roleId: item.Id);
            }

        });

    private ValueTask<Role> AddRoleValueAsync(Role newRole) =>
        AddRoleAsync(newRole: newRole);

    private ValueTask<Role> UpdateRoleValueAsync(Role updatedRole) =>
        UpdateRoleAsync(updatedRole: updatedRole);

    private ValueTask DeleteRoleValueAsync(Guid roleId) =>
        DeleteAsync(roleId: roleId);
}