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
    public async Task ShouldAddWithDataContextWhenLinkDoesNotExistForSaveAsync()
    {
        // Given
        User targetUser = new()
        {
            Id = "target-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Target",
            Email = "target@example.com",
            IsActive = true,
        };
        Role role = new()
        {
            Id = Guid.NewGuid(),
            AppId = 1,
            Name = "Editors",
            Privs = "page_read",
            Users = [],
            App = new cCoder.Data.Models.CMS.App
            {
                Id = 1,
                Name = "App",
                Domain = "app.local",
            },
        };
        UserRole link = new() { UserId = targetUser.Id, RoleId = role.Id };
        userRoleServiceMock.Setup(x => x.GetAll(true)).Returns(Array.Empty<UserRole>().AsQueryable());
        userRoleServiceMock.Setup(x => x.AddUserRoleAsync(link, false)).ReturnsAsync(link);

        // When
        UserRole result = await userRoleProcessingService.SaveUserRoleAsync(link);

        // Then
        Assert.Same(link, result);
        userRoleServiceMock.Verify(x => x.AddUserRoleAsync(link, false), Times.Once);
    }

}