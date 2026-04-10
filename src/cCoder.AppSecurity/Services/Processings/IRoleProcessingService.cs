using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IRoleProcessingService
{
    Role Get(Guid id);
    IQueryable<Role> GetAll(bool ignoreFilters = false);
    ValueTask<Role> AddAsync(Role entity);
    ValueTask<Role> AddValidatedAsync(Role entity);
    ValueTask<Role> UpdateAsync(Role entity);
    ValueTask<Role> UpdateValidatedAsync(Role entity);
    ValueTask DeleteAsync(Guid id);
    ValueTask<IEnumerable<Result<Role>>> AddOrUpdate(
        IEnumerable<Role> items
    );
    ValueTask DeleteAllAsync(IEnumerable<Role> items);
}









