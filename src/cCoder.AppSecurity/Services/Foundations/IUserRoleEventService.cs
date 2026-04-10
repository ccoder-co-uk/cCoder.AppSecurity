using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

public interface IUserRoleEventService
{
    ValueTask RaiseUserRoleAddEventAsync(UserRole entity);
    ValueTask RaiseUserRoleDeleteEventAsync(UserRole entity);
}









