// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldReturnUpdatedUserWhenUpdateUserFromAccountEventAsync()
    {
        // Given
        User user = CreateRandomUser();

        userServiceMock
            .Setup(expression: service => service.UpdateUserFromAccountEventAsync(
                user: user))
            .ReturnsAsync(value: user);

        // When
        User result = await userProcessingService
            .UpdateUserFromAccountEventAsync(updatedUser: user);

        // Then
        result
            .Should()
            .BeSameAs(expected: user);

        userServiceMock.Verify(
            expression: service => service.UpdateUserFromAccountEventAsync(
                user: user),
            times: Times.Once);
    }
}