// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Foundations;

public interface IAppService
{
    IQueryable<App> GetAll();

    App GetByDomain(string domain);
}