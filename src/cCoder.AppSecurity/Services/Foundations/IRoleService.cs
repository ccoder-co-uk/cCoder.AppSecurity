using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

public interface IRoleService
{
    Role Get(Guid id);
    IQueryable<Role> GetAll(bool ignoreFilters = false);
    ValueTask<Role> AddAsync(Role role);
    ValueTask<Role> AddValidatedAsync(Role role);
    ValueTask<Role> UpdateAsync(Role role);
    ValueTask<Role> UpdateValidatedAsync(Role role);
    ValueTask DeleteAsync(Guid id);
}









