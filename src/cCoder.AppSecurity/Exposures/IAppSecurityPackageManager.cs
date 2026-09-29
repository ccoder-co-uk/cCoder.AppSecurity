// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.AppSecurity.Models;


namespace cCoder.AppSecurity.Exposures;

public interface IAppSecurityPackageManager
{
    ValueTask ImportPackageAsync(int appId, AppSecurityPackage appSecurityPackage);

    AppSecurityPackage ExportPackage(int appId, string packageName);
}