// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Storages;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Foundations;

internal class AppService(IAppBroker appBroker) : IAppService
{
    public IQueryable<App> GetAll() =>
        appBroker.GetAll();

    public App GetByDomain(string domain) =>
        appBroker.GetByDomain(domain: domain);
}