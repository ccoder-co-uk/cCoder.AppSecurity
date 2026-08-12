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
    public void ShouldReturnUserWhenGetByEmail()
    {
        // Given
        User user = CreateRandomUser(email: "user@example.test");

        userServiceMock
            .Setup(expression: service => service.GetByEmail(
                email: user.Email,
                ignoreFilters: true))
            .Returns(value: user);

        // When
        User result = userProcessingService.GetByEmail(
            email: user.Email,
            ignoreFilters: true);

        // Then
        result
            .Should()
            .BeSameAs(expected: user);

        userServiceMock.Verify(
            expression: service => service.GetByEmail(
                email: user.Email,
                ignoreFilters: true),
            times: Times.Once);
    }
}