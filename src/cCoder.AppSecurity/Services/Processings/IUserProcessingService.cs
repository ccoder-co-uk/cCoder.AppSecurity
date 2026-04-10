using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IUserProcessingService
{
    User Get(string id);
    User GetByEmail(string email, bool ignoreFilters = false);
    IQueryable<User> GetAll(bool ignoreFilters = false);
    ValueTask<User> AddAsync(User entity);
    ValueTask<User> UpdateAsync(User entity);
    ValueTask DeleteAsync(string id);
    ValueTask<IEnumerable<Result<User>>> AddOrUpdate(
        IEnumerable<User> items
    );
    ValueTask DeleteAllAsync(IEnumerable<User> items);
}









