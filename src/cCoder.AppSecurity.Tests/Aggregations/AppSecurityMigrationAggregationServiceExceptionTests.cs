// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Aggregations;

public sealed partial class AppSecurityMigrationAggregationServiceExceptionTests
{
    private readonly Mock<IAppSecurityPackageOrchestrationService> packageServiceMock = new();
    private readonly Mock<IAppSecurityPackageExportOrchestrationService> exportServiceMock = new();
    private readonly Mock<IAppOrchestrationService> appServiceMock = new();
    private readonly Mock<IPageRoleOrchestrationService> pageRoleServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        new()
        {
            {
                new AppSecurityAggregationValidationException(
                    innerException: new ArgumentException()),
                typeof(AppSecurityAggregationValidationException)
            },
            {
                new AppSecurityAggregationDependencyException(
                    innerException: new InvalidOperationException()),
                typeof(AppSecurityAggregationDependencyException)
            },
            { new InvalidOperationException(), typeof(AppSecurityAggregationServiceException) }
        };

    [Fact]
    public async Task ShouldIgnorePackageWithoutSecurityItemsWhenImportPackageAsync()
    {
        // Given
        AppSecurityPackage package = new() { Items = [] };
        AppSecurityMigrationAggregationService service = CreateService();

        // When
        await service.ImportPackageAppSecurityPackageAsync(
            appId: 7,
            package: package);

        // Then
        packageServiceMock.VerifyNoOtherCalls();
        appServiceMock.VerifyNoOtherCalls();
        pageRoleServiceMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapImportPackageFailureAsync(Exception exception, Type expectedType)
    {
        // Given
        AppSecurityPackage package = new()
        {
            Items = [new AppSecurityPackageItem { Type = "AppSecurity/Role" }]
        };

        packageServiceMock
            .Setup(expression: service => service.MapAppSecurityPackageMappingRoles(
                mapping: It.Is<AppSecurityPackageMapping>(match: _ => true)))
            .Throws(exception: exception);

        AppSecurityMigrationAggregationService service = CreateService();

        // When
        Func<Task> action = async () => await service
            .ImportPackageAppSecurityPackageAsync(appId: 7, package: package);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapExportPackageFailure(Exception exception, Type expectedType)
    {
        // Given
        exportServiceMock
            .Setup(expression: service => service.ExportAppSecurityPackage(
                appId: 7,
                packageName: "baseline"))
            .Throws(exception: exception);

        AppSecurityMigrationAggregationService service = CreateService();

        // When
        Action action = () => service.ExportPackage(
            appId: 7,
            packageName: "baseline");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public void ShouldReturnPackageWhenExportPackage()
    {
        // Given
        AppSecurityPackage package = new();

        exportServiceMock
            .Setup(expression: service => service.ExportAppSecurityPackage(
                appId: 7,
                packageName: "baseline"))
            .Returns(value: package);

        AppSecurityMigrationAggregationService service = CreateService();

        // When
        AppSecurityPackage result = service.ExportPackage(
            appId: 7,
            packageName: "baseline");

        // Then
        result
            .Should()
            .BeSameAs(expected: package);
    }

    private AppSecurityMigrationAggregationService CreateService() =>
        new(
            packageOrchestrationService: packageServiceMock.Object,
            packageExportOrchestrationService: exportServiceMock.Object,
            appOrchestrationService: appServiceMock.Object,
            pageRoleOrchestrationService: pageRoleServiceMock.Object);
}