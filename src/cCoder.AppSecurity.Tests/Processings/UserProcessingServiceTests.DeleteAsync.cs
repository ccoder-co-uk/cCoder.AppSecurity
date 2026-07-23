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
        coreAuthInfoMock.SetupGet(x => x.SSOUserId).Returns(currentUser.Id);
        userServiceMock.Setup(x => x.Get(currentUser.Id)).Returns(currentUser);

        userServiceMock
            .Setup(x => x.DeleteAsync(currentUser.Id))
            .Returns(ValueTask.CompletedTask);

        // When
        await userProcessingService.DeleteAsync(currentUser.Id);

        // Then
        userServiceMock.Verify(x => x.DeleteAsync(currentUser.Id), Times.Once);
    }

    [Fact]
    public async Task ShouldThrowSecurityExceptionWhenDeletingDifferentUserForDeleteAsync()
    {
        // Given
        User targetUser = CreateRandomUser(id: "other-user", email: "other@example.com");
        coreAuthInfoMock.SetupGet(x => x.SSOUserId).Returns("different-user");
        userServiceMock.Setup(x => x.Get(targetUser.Id)).Returns(targetUser);

        // When
        await Assert.ThrowsAsync<SecurityException>(async () =>
            await userProcessingService.DeleteAsync(targetUser.Id)
        );

        // Then
    }

}