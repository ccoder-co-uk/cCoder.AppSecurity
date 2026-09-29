// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Brokers.Storages;

public interface IAppBroker
{
    IQueryable<App> GetAll();

    App GetByDomain(string domain);
}