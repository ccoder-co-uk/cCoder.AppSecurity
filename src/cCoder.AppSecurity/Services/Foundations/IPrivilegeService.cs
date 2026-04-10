using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

public interface IPrivilegeService
{
    Privilege Get(string id);
    IQueryable<Privilege> GetAll(bool ignoreFilters = false);
    ValueTask<Privilege> AddAsync(Privilege privilege);
    ValueTask<Privilege> UpdateAsync(Privilege privilege);
    ValueTask DeleteAsync(string id);
}









