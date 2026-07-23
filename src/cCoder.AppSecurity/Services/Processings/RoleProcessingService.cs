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
    public Role Get(Guid id) =>
        service.Get(id: id);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) =>
        service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Role> AddRoleAsync(Role entity) =>
        service.AddRoleAsync(role: entity);

    public ValueTask<Role> AddValidatedRoleAsync(Role entity) =>
        service.AddValidatedRoleAsync(role: entity);

    public ValueTask<Role> UpdateRoleAsync(Role entity) =>
        service.UpdateRoleAsync(role: entity);

    public ValueTask<Role> UpdateValidatedRoleAsync(Role entity) =>
        service.UpdateValidatedRoleAsync(role: entity);

    public ValueTask DeleteAsync(Guid id) =>
        service.DeleteAsync(id: id);

    public ValueTask DeleteValidatedAsync(Guid id) =>
        service.DeleteValidatedAsync(id: id);

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
    Item = isAdd ? await AddRoleAsync(entity: item) : await UpdateRoleAsync(entity: item),
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
    public async ValueTask DeleteAllRoleAsync(IEnumerable<Role> items)
    {
        foreach (Role item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }
}