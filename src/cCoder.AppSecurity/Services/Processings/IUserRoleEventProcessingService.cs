using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IUserRoleEventProcessingService
{
    ValueTask RaiseUserRoleAddEventAsync(UserRole entity);
    ValueTask RaiseUserRoleDeleteEventAsync(UserRole entity);
}








