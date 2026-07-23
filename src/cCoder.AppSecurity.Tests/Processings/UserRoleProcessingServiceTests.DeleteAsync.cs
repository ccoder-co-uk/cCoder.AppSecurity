// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationDeleteWhenUserCanDeleteUserRoleForDeleteAsync()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));
        User actor = WithPrivilege("userrole_delete", 1);
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
        };
        UserRole link = new()
        {
            UserId = "target-user",
            RoleId = role.Id,
            Role = role,
        };
        currentUser = actor;
        userRoleServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { link }.AsQueryable());
        userRoleServiceMock.Setup(x => x.DeleteAsync(link)).Returns(ValueTask.CompletedTask);

        // When
        await userRoleProcessingService.DeleteAsync(
            new UserRole { UserId = link.UserId, RoleId = link.RoleId }
        );

        // Then
        userRoleServiceMock.Verify(x => x.GetAll(true), Times.Once);
        userRoleServiceMock.Verify(
            x =>
                x.DeleteAsync(
                    It.Is<UserRole>(item =>
                        item.UserId == link.UserId && item.RoleId == link.RoleId
                    )
                ),
            Times.Once
        );
    }

}