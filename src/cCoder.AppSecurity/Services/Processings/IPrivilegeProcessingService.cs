using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IPrivilegeProcessingService
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddAsync(Privilege entity);
    ValueTask<Privilege> UpdateAsync(Privilege entity);
    ValueTask DeleteAsync(string id);
    ValueTask<IEnumerable<Result<Privilege>>> AddOrUpdate(
        IEnumerable<Privilege> items
    );
    ValueTask DeleteAllAsync(IEnumerable<Privilege> items);
}









