// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.CMS;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers.Storages;

internal sealed class AppBroker(ICoreContextFactory coreContextFactory) : IAppBroker
{
    public IQueryable<App> GetAll()
    {
        CoreDataContext context = coreContextFactory.CreateCoreContext();

        return context.Apps
            .IgnoreQueryFilters();
    }

    public App GetByDomain(string domain)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        return context.Apps
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: app => app.Domain == domain);
    }
}