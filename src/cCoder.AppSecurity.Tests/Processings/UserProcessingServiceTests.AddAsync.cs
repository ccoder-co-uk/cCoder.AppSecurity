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
        userServiceMock.Setup(x => x.GetAll(true)).Returns(Enumerable.Empty<User>().AsQueryable());
        userServiceMock.Setup(x => x.AddAsync(newUser)).ReturnsAsync(newUser);

        // When
        User result = await userProcessingService.AddAsync(newUser);

        // Then
        Assert.Same(newUser, result);
        userServiceMock.Verify(x => x.AddAsync(newUser), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnExistingUserWhenUserAlreadyExistsForAddAsync()
    {
        // Given
        User existingUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        User newUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { existingUser }.AsQueryable());

        // When
        User result = await userProcessingService.AddAsync(newUser);

        // Then
        Assert.Same(existingUser, result);
    }

    [Fact]
    public async Task ShouldReturnExistingUserWhenEmailAlreadyExistsForAddAsync()
    {
        // Given
        User existingUser = CreateRandomUser(id: "existing-user", email: "existing@example.com");
        User newUser = CreateRandomUser(id: "new-user", email: "existing@example.com");
        userServiceMock.Setup(x => x.GetAll(true)).Returns(new[] { existingUser }.AsQueryable());

        // When
        User result = await userProcessingService.AddAsync(newUser);

        // Then
        Assert.Same(existingUser, result);
    }

}