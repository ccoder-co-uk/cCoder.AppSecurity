// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseDataContextWhenUpdatingSelfForUpdateAsync()
    {
        // Given
        User user = CreateRandomUser(id: "test-user");
        coreAuthInfoMock.SetupGet(x => x.SSOUserId).Returns(user.Id);
        userServiceMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(user);

        // When
        User result = await userProcessingService.UpdateAsync(user);

        // Then
        Assert.Same(user, result);
        userServiceMock.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUpdatingDifferentUserForUpdateAsync()
    {
        // Given
        coreAuthInfoMock.SetupGet(x => x.SSOUserId).Returns("different-user");

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await userProcessingService.UpdateAsync(
                CreateRandomUser(id: "other-user", email: "other@example.com")
            )
        );

        // Then
    }

}