// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Processings;

internal class AppProcessingService(IAppService service) : IAppProcessingService
{
    public IQueryable<App> GetAll() => service.GetAll();

    public App GetByDomain(string domain) => service.GetByDomain(domain);
}