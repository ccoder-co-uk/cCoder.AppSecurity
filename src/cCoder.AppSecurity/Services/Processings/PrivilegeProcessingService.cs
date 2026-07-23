// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        authorizationBroker.Authorize(appId: null, privilege: "privilege_read");
        return service.Get(id: id);
    }

    public IQueryable<Privilege> GetAll(bool ignoreFilters = false)
    {
        authorizationBroker.Authorize(appId: null, privilege: "privilege_read");
        return service.GetAll(ignoreFilters: ignoreFilters);
    }

    public ValueTask<Privilege> AddPrivilegeAsync(Privilege entity)
    {
        authorizationBroker.Authorize(appId: null, privilege: "privilege_create");
        throw new InvalidOperationException(message: "Cannot add privileges");
    }

    public ValueTask<Privilege> UpdatePrivilegeAsync(Privilege entity)
    {
        authorizationBroker.Authorize(appId: null, privilege: "privilege_update");
        throw new InvalidOperationException(message: "Cannot update privileges");
    }

    public ValueTask DeleteAsync(string id)
    {
        authorizationBroker.Authorize(appId: null, privilege: "privilege_delete");
        throw new InvalidOperationException(message: "Cannot delete privileges");
    }

    public async ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdatePrivilege(
        IEnumerable<Privilege> items
    )
    {
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
    Item = isAdd ? await AddPrivilegeAsync(entity: item) : await UpdatePrivilegeAsync(entity: item),
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
    }
    public async ValueTask DeleteAllPrivilegeAsync(IEnumerable<Privilege> items)
    {
        foreach (Privilege item in items)
        {
            await DeleteAsync(id: item.Id);
        }
    }
}