// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseFoundationReadsAndAddAsyncWhenUserIsNewForAddOrUpdate()
    {
        // Given
        User user = CreateRandomUser(id: string.Empty, email: "new@example.com");
        IQueryable<User> allUsers = Array.Empty<User>().AsQueryable();

        userServiceMock.Setup(x => x.GetAll(true)).Returns(allUsers);

        userServiceMock.Setup(x => x.AddUserAsync(user)).ReturnsAsync(user);

        // When
        Result<User>[] results = (
            await userProcessingService.AddOrUpdateUser(new[] { user })
        ).ToArray();

        // Then
        results.Should().ContainSingle();
        results[0].Success.Should().BeTrue();
        results[0].Item.Should().BeSameAs(user);
        results[0].Message.Should().Be("Added Successfully");
        userServiceMock.Verify(x => x.GetAll(true), Times.Once);
        userServiceMock.Verify(x => x.AddUserAsync(user), Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}