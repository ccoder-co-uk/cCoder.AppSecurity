// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenAddAsync()
    {
        // Given
        Role expected = new()
        {
            Id = Guid.NewGuid(),
            Name = "Administrators",
            Privs = "app_read",
        };
        currentUser = new User
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles =
            [
                new UserRole
                {
                    UserId = "test-user",
                    RoleId = Guid.NewGuid(),
                    Role = new Role
                    {
                        Id = Guid.NewGuid(),
                        AppId = 1,
                        Name = "Test Role",
                        Privs = "role_create",
                        App = new App { Id = 1, Name = "App", Domain = "app.local" },
                        Users = [],
                        Pages = [],
                        Folders = [],
                    },
                },
            ],
        };
        roleServiceMock.Setup(x => x.AddAsync(expected)).ReturnsAsync(expected);

        // When
        Role actual = await roleProcessingService.AddAsync(expected);

        // Then
        Assert.Same(expected, actual);
        roleServiceMock.Verify(x => x.AddAsync(expected), Times.Once);
    }

}