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
    public async Task ShouldDelegateToFoundationServiceWhenDeletingSelfForDeleteAsync()
    {
        // Given
        User currentUser = CreateRandomUser();
        coreAuthInfoMock.SetupGet(expression: x => x.SSOUserId).Returns(value: currentUser.Id);
        userServiceMock.Setup(expression: x => x.Get(id: currentUser.Id)).Returns(value: currentUser);

        userServiceMock
            .Setup(expression: x => x.DeleteAsync(id: currentUser.Id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await userProcessingService.DeleteAsync(userId: currentUser.Id);

        // Then
        userServiceMock.Verify(expression: x => x.DeleteAsync(id: currentUser.Id), times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenDeletingDifferentUserForDeleteAsync()
    {
        // Given
        User targetUser = CreateRandomUser(id: "other-user", email: "other@example.com");
        coreAuthInfoMock.SetupGet(expression: x => x.SSOUserId).Returns(value: "different-user");
        userServiceMock.Setup(expression: x => x.Get(id: targetUser.Id)).Returns(value: targetUser);

        // When
        await Assert.ThrowsAsync<SecurityException>(testCode: async () =>
            await userProcessingService.DeleteAsync(userId: targetUser.Id)
        );

        // Then
    }

}