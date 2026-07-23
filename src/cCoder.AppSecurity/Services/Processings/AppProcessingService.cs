// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AppProcessingService(IAppService service) : IAppProcessingService
{
    public IQueryable<App> GetAll() =>
        TryCatch(operation: IQueryable<App> () =>
        {
            ValidateGetAll();

            return service.GetAll();
        });

    public App GetByDomain(string domain) =>
        TryCatch(operation: App () =>
        {
            ValidateGetByDomain(
                domain: domain);

            return service.GetByDomain(domain: domain);
        });
}