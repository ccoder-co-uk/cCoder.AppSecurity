// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldProcessWithoutThrowingWhenItemUsesCompositeKeyForAddOrUpdate()
    {
        // Given
        UserRole link = new() { UserId = "target-user", RoleId = Guid.NewGuid() };
        currentUser = WithoutPrivileges();
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));
        userRoleServiceMock.Setup(x => x.GetAll(false)).Returns(Array.Empty<UserRole>().AsQueryable());
        userRoleServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<UserRole>().AsQueryable());

        // When
        Func<Task> act = async () =>
            await userRoleProcessingService.AddOrUpdateUserRole(new[] { link });

        // Then
        await act.Should().NotThrowAsync();
    }

}