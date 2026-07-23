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
    public async Task ShouldAddNewUserWhenUserDoesNotExistForAddAsync()
    {
        // Given
        User newUser = CreateRandomUser(id: "new-user", email: "new@example.com");
        userServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true)).Returns(value: Enumerable.Empty<User>().AsQueryable());
        userServiceMock.Setup(expression: x => x.AddUserAsync(user: newUser)).ReturnsAsync(value: newUser);

        // When
        User result = await userProcessingService.AddUserAsync(newUser: newUser);

        // Then
        Assert.Same(expected: newUser, actual: result);
        userServiceMock.Verify(expression: x => x.AddUserAsync(user: newUser), times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnExistingUserWhenUserAlreadyExistsForAddAsync()
    {
        // Given
        User existingUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        User newUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        userServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true)).Returns(value: new[] { existingUser }.AsQueryable());

        // When
        User result = await userProcessingService.AddUserAsync(newUser: newUser);

        // Then
        Assert.Same(expected: existingUser, actual: result);
    }

    [Fact]
    public async Task ShouldReturnExistingUserWhenEmailAlreadyExistsForAddAsync()
    {
        // Given
        User existingUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        User newUser = CreateRandomUser(id: "new-user", email: "existing@example.com");
        userServiceMock.Setup(expression: x => x.GetAll(ignoreFilters: true)).Returns(value: new[] { existingUser }.AsQueryable());

        // When
        User result = await userProcessingService.AddUserAsync(newUser: newUser);

        // Then
        Assert.Same(expected: existingUser, actual: result);
    }

}