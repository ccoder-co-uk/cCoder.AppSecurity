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

        coreAuthInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: user.Id);

        userServiceMock.Setup(expression: x => x.UpdateUserAsync(user: user))
            .ReturnsAsync(value: user);

        // When
        User result = await userProcessingService.UpdateUserAsync(updatedUser: user);

        // Then
        Assert.Same(expected: user, actual: result);
        userServiceMock.Verify(expression: x => x.UpdateUserAsync(user: user), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenUpdatingDifferentUserForUpdateAsync()
    {
        // Given
        coreAuthInfoMock.SetupGet(expression: x => x.SSOUserId)
            .Returns(value: "different-user");

        // When
        await Assert.ThrowsAsync<cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException>(testCode: async () =>
            await userProcessingService.UpdateUserAsync(
updatedUser: CreateRandomUser(id: "other-user", email: "other@example.com")
            )
        );

        // Then
    }

}