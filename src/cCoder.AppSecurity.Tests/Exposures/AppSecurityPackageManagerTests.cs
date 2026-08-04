// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures;

public sealed partial class AppSecurityPackageManagerTests
{
    [Fact]
    public async Task ShouldDelegatePackageImportAndExportAsync()
    {
        // Given
        int appId = 731;
        string packageName = "security-package";
        var package = new AppSecurityPackage();

        var migrationServiceMock =
            new Mock<IAppSecurityMigrationAggregationService>(behavior: MockBehavior.Strict);

        migrationServiceMock
            .Setup(expression: service => service.ImportPackageAppSecurityPackageAsync(
                appId: appId,
                package: package))
            .Returns(value: ValueTask.CompletedTask);

        migrationServiceMock
            .Setup(expression: service => service.ExportPackage(
                appId: appId,
                packageName: packageName))
            .Returns(value: package);

        var manager = new AppSecurityPackageManager(
            appSecurityMigrationAggregationService: migrationServiceMock.Object);

        // When
        await manager.ImportPackageAsync(appId: appId, package: package);

        AppSecurityPackage actualPackage = manager.ExportPackage(
            appId: appId,
            packageName: packageName);

        // Then
        actualPackage.Should()
            .BeSameAs(expected: package);

        migrationServiceMock.Verify(
            expression: service => service.ImportPackageAppSecurityPackageAsync(
                appId: appId,
                package: package),
            times: Times.Once);

        migrationServiceMock.Verify(
            expression: service => service.ExportPackage(
                appId: appId,
                packageName: packageName),
            times: Times.Once);

        migrationServiceMock.VerifyNoOtherCalls();
    }
}