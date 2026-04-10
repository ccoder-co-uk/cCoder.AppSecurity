using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Aggregations;

public interface IAppSecurityMigrationAggregationService
{
    ValueTask ImportPackageAsync(int appId, AppSecurityPackage package);

    AppSecurityPackage ExportPackage(int appId, string packageName);
}

