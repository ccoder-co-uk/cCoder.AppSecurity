using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Foundations;

public interface IAppService
{
    App GetByDomain(string domain);
}
