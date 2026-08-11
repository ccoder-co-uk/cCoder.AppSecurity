// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Security;

[Collection(name: WebAcceptanceCollection.Name)]
public sealed partial class AppSecurityPackageTests(WebAcceptanceFixture fixture)
{
    [Fact]
    public async Task ShouldPersistFullPackageRelationshipsAfterPagesImportedAsync()
    {
        // Given
        int appId;
        int rootPageId;

        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            App app = await core.AddAppAsync(app: new App
            {
                Name = "Package acceptance",
                Domain = $"package-{Guid.NewGuid():N}.local",
                DefaultTheme = "Default",
                DefaultCultureId = string.Empty,
                TenantId = $"tenant-{Guid.NewGuid():N}",
                ConfigJson = "{}",
            });

            appId = app.Id;

            Page rootPage = await core.AddPageAsync(page: new Page
            {
                AppId = appId,
                Name = "Home",
                Path = string.Empty,
                ResourceKey = "Default",
                Layout = "Default",
                CreatedBy = "Acceptance",
                LastUpdatedBy = "Acceptance",
                CreatedOn = DateTimeOffset.UtcNow,
                LastUpdated = DateTimeOffset.UtcNow,
            });

            rootPageId = rootPage.Id;
        }

        AppSecurityPackage package = new()
        {
            Items =
            [
                new AppSecurityPackageItem
                {
                    Type = "ContentManagement/Page",
                    Data = "[{\"Path\":\"\"}]",
                },
                new AppSecurityPackageItem
                {
                    Type = "AppSecurity/Role",
                    Data = "[{\"Name\":\"Guests\",\"Privs\":\"\"}]",
                },
                new AppSecurityPackageItem
                {
                    Type = "ContentManagement/PageRole",
                    Data = "[{\"Path\":\"\",\"Role\":\"Guests\"}]",
                },
            ],
        };

        // When
        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            IAppSecurityPackageManager packageManager = scope.ServiceProvider
                .GetRequiredService<IAppSecurityPackageManager>();

            await packageManager.ImportPackageAsync(
                appId: appId,
                package: package);
        }

        // Then
        using (IServiceScope scope = fixture.Factory.Services.CreateScope())
        {
            using var core = scope.ServiceProvider
                .GetRequiredService<cCoder.Data.ICoreContextFactory>()
                .CreateCoreContext();

            Role guestRole = await core.Roles
                .IgnoreQueryFilters()
                .SingleAsync(predicate: role =>
                    role.AppId == appId && role.Name == "Guests");

            PageRole[] pageRoles = await core.PageRoles
                .IgnoreQueryFilters()
                .Where(predicate: pageRole =>
                    pageRole.RoleId == guestRole.Id
                    && pageRole.PageId == rootPageId)
                .ToArrayAsync();

            Assert.Single(collection: pageRoles);
        }
    }
}