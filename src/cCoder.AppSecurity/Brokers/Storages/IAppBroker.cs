using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Brokers.Storages;

public interface IAppBroker
{
    IQueryable<App> GetAll();

    App GetByDomain(string domain);
}
