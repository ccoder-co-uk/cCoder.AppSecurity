// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Models.Exceptions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public async Task ShouldAddMissingUserForAddFromAccountEventAsync()
    {
        // Given
        User user = CreateRandomUser(
            id: "event-user",
            email: "event@example.com");

        IQueryable<User> users = Queryable.AsQueryable(
            source: Array.Empty<User>());

        userServiceMock
            .Setup(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: users);

        userServiceMock
            .Setup(expression: service =>
                service.AddUserFromAccountEventAsync(user: user))
            .ReturnsAsync(value: user);

        // When
        User result = await userProcessingService
            .AddUserFromAccountEventAsync(newUser: user);

        // Then
        Assert.Same(expected: user, actual: result);

        userServiceMock.Verify(
            expression: service =>
                service.AddUserFromAccountEventAsync(user: user),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnConcurrentlyAddedUserForAddFromAccountEventAsync()
    {
        // Given
        User user = CreateRandomUser(
            id: "event-user",
            email: "event@example.com");

        IQueryable<User> noUsers = Queryable.AsQueryable(
            source: Array.Empty<User>());

        IQueryable<User> addedUsers = Queryable.AsQueryable(
            source: new[] { user });

        userServiceMock
            .SetupSequence(expression: service => service.GetAll(
                ignoreFilters: true))
            .Returns(value: noUsers)
            .Returns(value: addedUsers);

        userServiceMock
            .Setup(expression: service =>
                service.AddUserFromAccountEventAsync(user: user))
            .ThrowsAsync(exception: new AppSecurityServiceException(
                innerException: new InvalidOperationException()));

        // When
        User result = await userProcessingService
            .AddUserFromAccountEventAsync(newUser: user);

        // Then
        Assert.Same(expected: user, actual: result);

        userServiceMock.Verify(
            expression: service => service.GetAll(
                ignoreFilters: true),
            times: Times.Exactly(callCount: 2));
    }
}