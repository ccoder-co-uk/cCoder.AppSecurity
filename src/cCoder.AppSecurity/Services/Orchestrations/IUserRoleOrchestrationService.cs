using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IUserRoleOrchestrationService
{
    IQueryable<UserRole> GetAll(bool ignoreFilters = false);
    ValueTask<UserRole> AddAsync(UserRole entity);
    ValueTask DeleteAsync(UserRole entity);
    ValueTask<IEnumerable<Result<UserRole>>> AddOrUpdate(
        IEnumerable<UserRole> items
    );
    ValueTask DeleteAllAsync(IEnumerable<UserRole> items);
    ValueTask<UserRole> SaveAsync(UserRole entity);
}








