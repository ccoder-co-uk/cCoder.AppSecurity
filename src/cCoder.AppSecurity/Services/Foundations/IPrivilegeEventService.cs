using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations.Events;

public interface IPrivilegeEventService
{
    ValueTask RaisePrivilegeAddEventAsync(Privilege entity);
    ValueTask RaisePrivilegeUpdateEventAsync(Privilege entity);
    ValueTask RaisePrivilegeDeleteEventAsync(Privilege entity);
}









