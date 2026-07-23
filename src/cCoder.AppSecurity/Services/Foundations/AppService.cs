// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AppService(IAppBroker appBroker) : IAppService
{
    public IQueryable<App> GetAll() =>
        TryCatch(operation: IQueryable<App> () =>
        {
            ValidateAllOnGet();

            return appBroker.GetAll();
        });

    public App GetByDomain(string domain) =>
        TryCatch(operation: App () =>
        {
            ValidateByDomainOnGet(
                domain: domain);

            return appBroker.GetByDomain(domain: domain);
        });
}