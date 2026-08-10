// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using DataPageRole = cCoder.Data.Models.Security.PageRole;
using DataRole = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldPersistRootPageRoleAfterRoleIsPersistedAsync()
    {
        // Given
        const int appId = 7;
        const int rootPageId = 11;
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: appId);

        role.Pages =
        [
            new DataPageRole
            {
                Page = new Page
                {
                    AppId = appId,
                    Path = string.Empty,
                },
            },
        ];

        MockSequence sequence = new();

        roleBrokerMock.InSequence(sequence: sequence)
            .Setup(expression: broker => broker.UpdateRoleAsync(
                entity: It.IsAny<DataRole>()))
            .ReturnsAsync(value: ToExternalRole(item: role));

        roleBrokerMock.InSequence(sequence: sequence)
            .Setup(expression: broker => broker.GetPageRolesByRoleId(
                roleId: roleId))
            .Returns(value: Array.Empty<DataPageRole>()
                .AsQueryable());

        roleBrokerMock.InSequence(sequence: sequence)
            .Setup(expression: broker => broker.GetPageIdByPath(
                appId: appId,
                path: string.Empty))
            .Returns(value: rootPageId);

        roleBrokerMock.InSequence(sequence: sequence)
            .Setup(expression: broker => broker.AddPageRoleAsync(
                newPageRole: It.Is<DataPageRole>(match: pageRole =>
                    pageRole.RoleId == roleId
                    && pageRole.PageId == rootPageId)))
            .ReturnsAsync(value: new DataPageRole
            {
                RoleId = roleId,
                PageId = rootPageId,
            });

        // When
        _ = await roleService.UpdateValidatedRoleAsync(updatedRole: role);

        // Then
        roleBrokerMock.Verify(
            expression: broker => broker.UpdateRoleAsync(
                entity: It.IsAny<DataRole>()),
            times: Times.Once);

        roleBrokerMock.Verify(
            expression: broker => broker.GetPageRolesByRoleId(
                roleId: roleId),
            times: Times.Once);

        roleBrokerMock.Verify(
            expression: broker => broker.GetPageIdByPath(
                appId: appId,
                path: string.Empty),
            times: Times.Once);

        roleBrokerMock.Verify(
            expression: broker => broker.AddPageRoleAsync(
                newPageRole: It.Is<DataPageRole>(match: pageRole =>
                    pageRole.RoleId == roleId
                    && pageRole.PageId == rootPageId)),
            times: Times.Once);

        roleBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldNotDuplicatePageRoleWhenUpdateIsRedeliveredAsync()
    {
        // Given
        const int appId = 7;
        const int pageId = 13;
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: appId);
        role.Pages = [new DataPageRole { PageId = pageId }];

        roleBrokerMock
            .Setup(expression: broker => broker.UpdateRoleAsync(
                entity: It.IsAny<DataRole>()))
            .ReturnsAsync(value: ToExternalRole(item: role));

        roleBrokerMock
            .Setup(expression: broker => broker.GetPageRolesByRoleId(
                roleId: roleId))
            .Returns(value: new[]
            {
                new DataPageRole
                {
                    RoleId = roleId,
                    PageId = pageId,
                },
            }.AsQueryable());

        // When
        _ = await roleService.UpdateValidatedRoleAsync(updatedRole: role);
        _ = await roleService.UpdateValidatedRoleAsync(updatedRole: role);

        // Then
        roleBrokerMock.Verify(
            expression: broker => broker.UpdateRoleAsync(
                entity: It.IsAny<DataRole>()),
            times: Times.Exactly(callCount: 2));

        roleBrokerMock.Verify(
            expression: broker => broker.GetPageRolesByRoleId(
                roleId: roleId),
            times: Times.Exactly(callCount: 2));

        roleBrokerMock.Verify(
            expression: broker => broker.AddPageRoleAsync(
                newPageRole: It.IsAny<DataPageRole>()),
            times: Times.Never);

        roleBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenPagePathCannotBeResolvedAsync()
    {
        // Given
        const int appId = 7;
        const string missingPath = "/missing";
        Guid roleId = Guid.NewGuid();
        Role role = CreateRandomRole(id: roleId, appId: appId);

        role.Pages =
        [
            new DataPageRole
            {
                Page = new Page
                {
                    AppId = appId,
                    Path = missingPath,
                },
            },
        ];

        roleBrokerMock
            .Setup(expression: broker => broker.UpdateRoleAsync(
                entity: It.IsAny<DataRole>()))
            .ReturnsAsync(value: ToExternalRole(item: role));

        roleBrokerMock
            .Setup(expression: broker => broker.GetPageRolesByRoleId(
                roleId: roleId))
            .Returns(value: Array.Empty<DataPageRole>()
                .AsQueryable());

        roleBrokerMock
            .Setup(expression: broker => broker.GetPageIdByPath(
                appId: appId,
                path: missingPath))
            .Returns(value: 0);

        // When
        Func<Task> action = async () =>
            await roleService.UpdateValidatedRoleAsync(updatedRole: role);

        // Then
        await action.Should()
            .ThrowAsync<AppSecurityValidationException>()
            .WithInnerException<AppSecurityValidationException, ArgumentException>(
                because: string.Empty,
                becauseArgs: []);

        roleBrokerMock.Verify(
            expression: broker => broker.AddPageRoleAsync(
                newPageRole: It.IsAny<DataPageRole>()),
            times: Times.Never);
    }
}