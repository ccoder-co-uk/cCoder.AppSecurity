// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class PageRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldPersistRootGuestPageRoleOnlyOnceWhenRedeliveredAsync()
    {
        // Given
        const int appId = 7;
        const int rootPageId = 11;
        Guid roleId = Guid.NewGuid();
        List<PageRole> pageRoles = [];
        Mock<IPageRoleService> pageRoleServiceMock = new(behavior: MockBehavior.Strict);

        Role guestRole = new()
        {
            AppId = appId,
            Id = roleId,
            Name = "Guests",
            Pages =
            [
                new PageRole
                {
                    Page = new Page { AppId = appId, Path = string.Empty },
                },
            ],
        };

        pageRoleServiceMock
            .Setup(expression: service => service.GetAll())
            .Returns(valueFunction: () => pageRoles.AsQueryable());

        pageRoleServiceMock
            .Setup(expression: service => service.GetPageId(
                appId: appId,
                path: string.Empty))
            .Returns(value: rootPageId);

        pageRoleServiceMock
            .Setup(expression: service => service.AddPageRoleAsync(
                newPageRole: It.IsAny<PageRole>()))
            .ReturnsAsync(valueFunction: (PageRole pageRole) =>
            {
                pageRoles.Add(item: pageRole);
                return pageRole;
            });

        PageRoleProcessingService processingService = new(
            pageRoleService: pageRoleServiceMock.Object);

        // When
        await processingService.AddOrUpdatePageRolesAsync(roles: [guestRole]);
        await processingService.AddOrUpdatePageRolesAsync(roles: [guestRole]);

        // Then
        Assert.Single(collection: pageRoles);
        Assert.Equal(expected: roleId, actual: pageRoles.Single().RoleId);
        Assert.Equal(expected: rootPageId, actual: pageRoles.Single().PageId);
    }

    [Fact]
    public async Task ShouldThrowWhenPageRolePathCannotBeResolvedAsync()
    {
        // Given
        const int appId = 7;

        Mock<IPageRoleService> pageRoleServiceMock = new(behavior: MockBehavior.Strict);

        Role role = new()
        {
            AppId = appId,
            Id = Guid.NewGuid(),
            Pages = [new PageRole { Page = new Page { Path = "/missing" } }],
        };

        pageRoleServiceMock
            .Setup(expression: service => service.GetAll())
            .Returns(value: Array.Empty<PageRole>()
                .AsQueryable());

        pageRoleServiceMock
            .Setup(expression: service => service.GetPageId(
                appId: appId,
                path: "/missing"))
            .Returns(value: 0);

        PageRoleProcessingService processingService = new(
            pageRoleService: pageRoleServiceMock.Object);

        // When
        async Task AddPageRoleAsync() =>
            await processingService.AddOrUpdatePageRolesAsync(roles: [role]);

        // Then
        _ = await Assert.ThrowsAsync<AppSecurityProcessingValidationException>(
            testCode: AddPageRoleAsync);
    }
}