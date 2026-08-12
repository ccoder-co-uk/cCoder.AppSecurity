// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations;

public sealed partial class AppSecurityPackageExportOrchestrationServiceTests
{
    private readonly Mock<IRoleProcessingService> roleProcessingServiceMock = new();
    private readonly Mock<IJsonProcessingService> jsonProcessingServiceMock = new();

    [Fact]
    public void ShouldExportOnlyRolesForRequestedApp()
    {
        // Given
        roleProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[]
            {
                new Role { AppId = 7, Name = "Users", Privs = "page_read" },
                new Role { AppId = 8, Name = "Other", Privs = "page_write" }
            }.AsQueryable());

        jsonProcessingServiceMock
            .Setup(expression: service => service.Serialize(value: It.IsAny<object>()))
            .Returns(value: "roles-json");

        AppSecurityPackageExportOrchestrationService service = CreateService();

        // When
        AppSecurityPackage result = service.ExportAppSecurityPackage(
            appId: 7,
            packageName: "Roles");

        // Then
        result.Name
            .Should()
            .Be(expected: "Roles");

        result.Items
            .Should()
            .ContainSingle();

        result.Items.Single().Data
            .Should()
            .Be(expected: "roles-json");
    }

    [Fact]
    public void ShouldReturnEmptyPackageForUnknownPackageName()
    {
        // Given
        AppSecurityPackageExportOrchestrationService service = CreateService();

        // When
        AppSecurityPackage result = service.ExportAppSecurityPackage(
            appId: 7,
            packageName: "Unknown");

        // Then
        result.Items
            .Should()
            .BeEmpty();
    }

    [Theory]
    [MemberData(
        nameof(PrivilegeOrchestrationServiceExceptionTests.ExceptionMappings),
        MemberType = typeof(PrivilegeOrchestrationServiceExceptionTests))]
    public void ShouldMapExportFailure(Exception exception, Type expectedType)
    {
        // Given
        roleProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Throws(exception: exception);

        AppSecurityPackageExportOrchestrationService service = CreateService();

        // When
        Action action = () => service.ExportAppSecurityPackage(
            appId: 7,
            packageName: "Roles");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AppSecurityPackageExportOrchestrationService CreateService() =>
        new(
            roleProcessingService: roleProcessingServiceMock.Object,
            jsonProcessingService: jsonProcessingServiceMock.Object);
}