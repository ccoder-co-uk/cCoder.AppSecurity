using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;


namespace cCoder.AppSecurity.Services.Processings;

internal class PrivilegeProcessingService(
    IPrivilegeService service,
    IAuthorizationBroker authorizationBroker
) : IPrivilegeProcessingService
{
    public Privilege Get(string id)
    {
        authorizationBroker.Authorize(null, "privilege_read");
        return service.Get(id);
    }

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false)
    {
        authorizationBroker.Authorize(null, "privilege_read");
        return service.GetAll(ignoreFilters);
    }

    public ValueTask<Privilege> AddAsync(Privilege entity)
    {
        authorizationBroker.Authorize(null, "privilege_create");
        throw new InvalidOperationException("Cannot add privileges");
    }

    public ValueTask<Privilege> UpdateAsync(Privilege entity)
    {
        authorizationBroker.Authorize(null, "privilege_update");
        throw new InvalidOperationException("Cannot update privileges");
    }

    public ValueTask DeleteAsync(string id)
    {
        authorizationBroker.Authorize(null, "privilege_delete");
        throw new InvalidOperationException("Cannot delete privileges");
    }

    public async ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdate(
        IEnumerable<Privilege> items
    )
    {
        List<Result<Privilege>> results = [];

        foreach (Privilege item in items)
        {
            try
            {
                bool isAdd = string.IsNullOrWhiteSpace(item.Id);

                results.Add(
                    new Result<Privilege>
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
                    new Result<Privilege>
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
    public async ValueTask DeleteAllAsync(IEnumerable<Privilege> items)
    {
        foreach (Privilege item in items)
            await DeleteAsync(item.Id);
    }
}














