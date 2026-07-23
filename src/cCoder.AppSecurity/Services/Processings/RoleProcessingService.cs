// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;


namespace cCoder.AppSecurity.Services.Processings;

internal class RoleProcessingService(IRoleService service) : IRoleProcessingService
{
    public Role Get(Guid roleId) =>
        service.Get(id: roleId);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Role> AddRoleAsync(Role newRole) =>
        service.AddRoleAsync(role: newRole);

    public ValueTask<Role> AddValidatedRoleAsync(Role newRole) =>
        service.AddValidatedRoleAsync(role: newRole);

    public ValueTask<Role> UpdateRoleAsync(Role updatedRole) =>
        service.UpdateRoleAsync(role: updatedRole);

    public ValueTask<Role> UpdateValidatedRoleAsync(Role updatedRole) =>
        service.UpdateValidatedRoleAsync(role: updatedRole);

    public ValueTask DeleteAsync(Guid roleId) =>
        service.DeleteAsync(id: roleId);

    public ValueTask DeleteValidatedAsync(Guid roleId) =>
        service.DeleteValidatedAsync(id: roleId);

    public async ValueTask<IEnumerable<Result<Role>>> AddOrUpdateRole(
        IEnumerable<Role> items
    )
    {
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
    Item = isAdd ? await AddRoleAsync(newRole: item) : await UpdateRoleAsync(updatedRole: item),
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
    }
    public async ValueTask DeleteAllRoleAsync(IEnumerable<Role> deletedRole)
    {
        foreach (Role item in deletedRole)
        {
            await DeleteAsync(roleId: item.Id);
        }
    }
}