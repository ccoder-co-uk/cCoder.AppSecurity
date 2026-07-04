using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Processings;

public interface IAppProcessingService
{
    IQueryable<App> GetAll();

    App GetByDomain(string domain);
}
