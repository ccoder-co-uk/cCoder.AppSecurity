// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldDeleteEachUserWhenSsoUserMatchesForDeleteAllAsync()
    {
        // Given
        User user = CreateRandomUser(id: "test-user");
        coreAuthInfoMock.SetupGet(expression: x => x.SSOUserId).Returns(value: user.Id);
        userServiceMock.Setup(expression: x => x.Get(id: user.Id)).Returns(value: user);
        userServiceMock.Setup(expression: x => x.DeleteAsync(id: user.Id)).Returns(value: ValueTask.CompletedTask);

        // When
        await userProcessingService.DeleteAllUserAsync(deletedUser: new[] { user });

        // Then
        userServiceMock.Verify(expression: x => x.Get(id: user.Id), times: Times.Once);
        userServiceMock.Verify(expression: x => x.DeleteAsync(id: user.Id), times: Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyGet(expression: x => x.SSOUserId, times: Times.Once);
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}