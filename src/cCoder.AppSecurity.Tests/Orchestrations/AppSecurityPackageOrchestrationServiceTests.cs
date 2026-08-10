// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Orchestrations;

public sealed partial class AppSecurityPackageOrchestrationServiceTests
{
    [Fact]
    public void ShouldMapRootGuestRelationshipFromPageRoleOnlyPackage()
    {
        // Given
        const int appId = 7;
        const string pageRoleJson = "[{\"Path\":\"\",\"Role\":\"Guests\"}]";

        Role persistedGuestRole = new()
        {
            AppId = appId,
            Id = Guid.NewGuid(),
            Name = "Guests",
        };

        AppSecurityPackage package = new()
        {
            Items =
            [
                new AppSecurityPackageItem
                {
                    Type = "ContentManagement/PageRole",
                    Data = pageRoleJson,
                },
            ],
        };

        Mock<IRoleProcessingService> roleProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

        Mock<IJsonProcessingService> jsonProcessingServiceMock =
            new(behavior: MockBehavior.Strict);

        roleProcessingServiceMock
            .Setup(expression: service => service.GetAll(ignoreFilters: true))
            .Returns(value: new[] { persistedGuestRole }
                .AsQueryable());

        jsonProcessingServiceMock
            .Setup(expression: service => service.ParseJson<PageRoleInfo[]>(
                json: pageRoleJson))
            .Returns(value:
            [
                new PageRoleInfo { Path = string.Empty, Role = "Guests" },
            ]);

        AppSecurityPackageOrchestrationService orchestrationService = new(
            roleProcessingService: roleProcessingServiceMock.Object,
            jsonProcessingService: jsonProcessingServiceMock.Object);

        AppSecurityPackageMapping mapping = new()
        {
            AppId = appId,
            Package = package,
        };

        // When
        AppSecurityPackageMapping result = orchestrationService
            .MapAppSecurityPackageMappingPageRoles(mapping: mapping);

        // Then
        Role guestRole = Assert.Single(collection: result.App.Roles);
        PageRole pageRole = Assert.Single(collection: guestRole.Pages);
        Assert.Equal(expected: persistedGuestRole.Id, actual: guestRole.Id);
        Assert.Equal(expected: string.Empty, actual: pageRole.Page.Path);
    }
}