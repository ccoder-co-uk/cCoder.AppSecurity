// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;
using DataRole = cCoder.Data.Models.Security.Role;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldMapPageAndFolderRelationshipsWhenDeleteValidatedRoleAsync()
    {
        // Given
        Role role = CreateRandomRole();

        Guid folderId = Guid.NewGuid();

        role.Pages = [new PageRole { PageId = 11, RoleId = role.Id }];

        role.Folders = [new FolderRole { FolderId = folderId, RoleId = role.Id }];

        DataRole captured = null;

        IQueryable<UserRole> noUserRoles = Array.Empty<UserRole>()
            .AsQueryable();

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: true))
            .Returns(value: new[] { role }.AsQueryable());

        userRoleBrokerMock
            .Setup(expression: broker => broker.GetAllUserRoles(ignoreFilters: true))
            .Returns(value: noUserRoles);

        roleBrokerMock
            .Setup(expression: broker => broker.DeletePageRolesByRoleIdAsync(
                roleId: role.Id))
            .Returns(value: ValueTask.CompletedTask);

        roleBrokerMock
            .Setup(expression: broker => broker.DeleteFolderRolesByRoleIdAsync(
                roleId: role.Id))
            .Returns(value: ValueTask.CompletedTask);

        roleBrokerMock
            .Setup(expression: broker => broker.DeleteRoleAsync(
                entity: It.Is<DataRole>(match: _ => true)))
            .Callback<DataRole>(action: submitted => captured = submitted)
            .Returns(value: new ValueTask<int>(result: 1));

        // When
        await roleService.DeleteValidatedAsync(roleId: role.Id);

        // Then
        captured.Pages
            .Should()
            .ContainSingle()
            .Which.PageId
            .Should()
            .Be(expected: 11);

        captured.Folders
            .Should()
            .ContainSingle()
            .Which.FolderId
            .Should()
            .Be(expected: folderId);
    }
}