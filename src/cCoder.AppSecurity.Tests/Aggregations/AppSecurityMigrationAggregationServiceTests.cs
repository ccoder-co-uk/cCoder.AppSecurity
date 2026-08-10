// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Aggregations;

public sealed partial class AppSecurityMigrationAggregationServiceTests
{
    [Fact]
    public async Task ShouldSkipPageRolesDuringGenericPackageImportAsync()
    {
        // Given
        AppSecurityPackage package = CreatePageRoleOnlyPackage();

        AppSecurityMigrationAggregationService aggregationService = CreateAggregationService(
            packageOrchestrationServiceMock: out Mock<IAppSecurityPackageOrchestrationService> packageMock,
            pageRoleOrchestrationServiceMock: out Mock<IPageRoleOrchestrationService> pageRoleMock);

        // When
        await aggregationService.ImportPackageAppSecurityPackageAsync(appId: 7, package: package);

        // Then
        packageMock.VerifyNoOtherCalls();
        pageRoleMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldProcessPageRolesDuringContentPagesImportedAsync()
    {
        // Given
        const int appId = 7;
        AppSecurityPackage package = CreatePageRoleOnlyPackage();
        App app = new() { Id = appId };

        AppSecurityMigrationAggregationService aggregationService = CreateAggregationService(
            packageOrchestrationServiceMock: out Mock<IAppSecurityPackageOrchestrationService> packageMock,
            pageRoleOrchestrationServiceMock: out Mock<IPageRoleOrchestrationService> pageRoleMock);

        packageMock
            .Setup(expression: service => service.MapAppSecurityPackageMappingPageRoles(
                mapping: It.Is<AppSecurityPackageMapping>(match: mapping =>
                    mapping.AppId == appId
                    && mapping.Package == package)))
            .Returns(value: new AppSecurityPackageMapping
            {
                AppId = appId,
                Package = package,
                App = app,
            });

        pageRoleMock
            .Setup(expression: service => service.AddOrUpdateAppPageRolesAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await aggregationService.ImportPageRolesAppSecurityPackageAsync(
            appId: appId,
            package: package);

        // Then
        packageMock.VerifyAll();
        pageRoleMock.VerifyAll();
    }

    private static AppSecurityMigrationAggregationService CreateAggregationService(
        out Mock<IAppSecurityPackageOrchestrationService> packageOrchestrationServiceMock,
        out Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock)
    {
        packageOrchestrationServiceMock = new(behavior: MockBehavior.Strict);
        pageRoleOrchestrationServiceMock = new(behavior: MockBehavior.Strict);

        return new AppSecurityMigrationAggregationService(
            packageOrchestrationService: packageOrchestrationServiceMock.Object,
            packageExportOrchestrationService: Mock.Of<IAppSecurityPackageExportOrchestrationService>(),
            appOrchestrationService: Mock.Of<IAppOrchestrationService>(),
            pageRoleOrchestrationService: pageRoleOrchestrationServiceMock.Object);
    }

    private static AppSecurityPackage CreatePageRoleOnlyPackage() =>
        new()
        {
            Items =
            [
                new AppSecurityPackageItem
                {
                    Type = "ContentManagement/PageRole",
                    Data = "[{\"Path\":\"\",\"Role\":\"Guests\"}]",
                },
            ],
        };
}