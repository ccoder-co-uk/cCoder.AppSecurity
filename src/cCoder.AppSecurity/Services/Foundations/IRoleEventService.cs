using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

public interface IRoleEventService
{
    ValueTask RaiseRoleAddEventAsync(Role entity);
    ValueTask RaiseRoleUpdateEventAsync(Role entity);
    ValueTask RaiseRoleDeleteEventAsync(Role entity);
}









