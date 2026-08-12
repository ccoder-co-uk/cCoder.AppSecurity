// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class UserControllerExceptionTests
{
    private readonly Mock<IUserManager> userManagerMock = new();
    private readonly Mock<ICoreAuthInfo> authInfoMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetMeFails()
    {
        // Given
#pragma warning disable STXFORMAT008
        authInfoMock
            .SetupGet(expression: auth => auth.SSOUserId)
            .Returns(value: "user-one");
#pragma warning restore STXFORMAT008
        UserController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.Get(id: "user-one"))
                .Throws(exception: exception);

            return controller.GetMe();
        });
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        UserController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetFails()
    {
        // Given
        UserController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.Get(id: "user-one"))
                .Throws(exception: exception);

            return controller.Get(key: "user-one");
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPostFailsAsync()
    {
        // Given
        User user = new() { Id = "user-one" };
        UserController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.AddUserAsync(entity: user))
                .Throws(exception: exception);

            return controller.Post(newUser: user);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPutFailsAsync()
    {
        // Given
        User user = new() { Id = "user-one" };
        UserController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.UpdateUserAsync(entity: user))
                .Throws(exception: exception);

            return controller.Put(key: "user-one", updatedUser: user);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPatchFailsAsync()
    {
        // Given
        UserController controller = CreateController();
        Delta<User> delta = new();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.Get(id: "user-one"))
                .Throws(exception: exception);

            return controller.Put(key: "user-one", updatedDelta: delta);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenDeleteFailsAsync()
    {
        // Given
        UserController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userManagerMock.Reset();

            userManagerMock
                .Setup(expression: manager => manager.DeleteAsync(id: "user-one"))
                .Throws(exception: exception);

            return controller.Delete(key: "user-one");
        });
    }

    private UserController CreateController() =>
        new(
            service: userManagerMock.Object,
            authInfo: authInfoMock.Object,
            loggingBroker: loggingBrokerMock.Object);
}