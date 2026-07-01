using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Brokers.Storages;

public interface IAppBroker
{
    App GetByDomain(string domain);
}
