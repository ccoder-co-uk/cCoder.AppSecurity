using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IUserEventProcessingService
{
    ValueTask RaiseUserAddEventAsync(User entity);
    ValueTask RaiseUserUpdateEventAsync(User entity);
    ValueTask RaiseUserDeleteEventAsync(User entity);
}








