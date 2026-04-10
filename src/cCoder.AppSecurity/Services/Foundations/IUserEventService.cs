using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

public interface IUserEventService
{
    ValueTask RaiseUserAddEventAsync(User entity);
    ValueTask RaiseUserUpdateEventAsync(User entity);
    ValueTask RaiseUserDeleteEventAsync(User entity);
}









