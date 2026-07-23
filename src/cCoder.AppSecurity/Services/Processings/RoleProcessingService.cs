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
    public Role Get(Guid id) => service.Get(id: id);

    public IQueryable<Role> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters: ignoreFilters);

    public ValueTask<Role> AddAsync(Role entity) => service.AddAsync(role: entity);

    public ValueTask<Role> AddValidatedAsync(Role entity) => service.AddValidatedAsync(role: entity);

    public ValueTask<Role> UpdateAsync(Role entity) => service.UpdateAsync(role: entity);

    public ValueTask<Role> UpdateValidatedAsync(Role entity) => service.UpdateValidatedAsync(role: entity);

    public ValueTask DeleteAsync(Guid id) => service.DeleteAsync(id: id);

    public ValueTask DeleteValidatedAsync(Guid id) => service.DeleteValidatedAsync(id: id);

    public async ValueTask<IEnumerable<Result<Role>>> AddOrUpdate(
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
    Item = isAdd ? await AddAsync(item) : await UpdateAsync(item),
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
    public async ValueTask DeleteAllAsync(IEnumerable<Role> items)
    {
        foreach (Role item in items)
            await DeleteAsync(id: item.Id);
    }
}