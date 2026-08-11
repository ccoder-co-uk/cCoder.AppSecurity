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
    public async Task ShouldImportFullPackageRelationshipsInOrderAfterPagesAsync()
    {
        // Given
        const int appId = 7;
        AppSecurityPackage package = CreateFullPackage();
        App roleApp = new() { Id = appId };
        App pageRoleApp = new() { Id = appId };
        Mock<IAppSecurityPackageOrchestrationService> packageMock = new(behavior: MockBehavior.Strict);
        Mock<IAppOrchestrationService> appMock = new(behavior: MockBehavior.Strict);
        Mock<IPageRoleOrchestrationService> pageRoleMock = new(behavior: MockBehavior.Strict);
        MockSequence sequence = new();

        packageMock.InSequence(sequence: sequence)
            .Setup(expression: service => service.MapAppSecurityPackageMappingRoles(
                mapping: It.Is<AppSecurityPackageMapping>(match: mapping =>
                    mapping.AppId == appId && mapping.Package == package)))
            .Returns(value: new AppSecurityPackageMapping { App = roleApp });

        appMock.InSequence(sequence: sequence)
            .Setup(expression: service => service.UpdateAppAsync(app: roleApp))
            .Returns(value: ValueTask.CompletedTask);

        packageMock.InSequence(sequence: sequence)
            .Setup(expression: service => service.MapAppSecurityPackageMappingPageRoles(
                mapping: It.Is<AppSecurityPackageMapping>(match: mapping =>
                    mapping.AppId == appId && mapping.Package == package)))
            .Returns(value: new AppSecurityPackageMapping { App = pageRoleApp });

        pageRoleMock.InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdateAppPageRolesAsync(app: pageRoleApp))
            .Returns(value: ValueTask.CompletedTask);

        AppSecurityMigrationAggregationService aggregationService = new(
            packageOrchestrationService: packageMock.Object,
            packageExportOrchestrationService: Mock.Of<IAppSecurityPackageExportOrchestrationService>(),
            appOrchestrationService: appMock.Object,
            pageRoleOrchestrationService: pageRoleMock.Object);

        // When
        await aggregationService.ImportPackageAppSecurityPackageAsync(
            appId: appId,
            package: package);

        // Then
        packageMock.VerifyAll();
        appMock.VerifyAll();
        pageRoleMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldProcessPageRolesDuringNormalPackageImportAsync()
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
        await aggregationService.ImportPackageAppSecurityPackageAsync(
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

    private static AppSecurityPackage CreateFullPackage() =>
        new()
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
                    Data = "[{\"Name\":\"Guests\"}]",
                },
                new AppSecurityPackageItem
                {
                    Type = "ContentManagement/PageRole",
                    Data = "[{\"Path\":\"\",\"Role\":\"Guests\"}]",
                },
            ],
        };
}