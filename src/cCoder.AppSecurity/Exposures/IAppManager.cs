// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Exposures;

public interface IAppManager
{
    IQueryable<App> GetAll();

    App GetByDomain(string domain);
}
