// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class UserRoleControllerExceptionTests
{
    private readonly Mock<IUserRoleManager> userRoleManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        UserRoleController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            userRoleManagerMock.Reset();

            userRoleManagerMock
                .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPostFailsAsync()
    {
        // Given
        UserRole userRole = new() { RoleId = Guid.NewGuid(), UserId = "user-one" };
        UserRoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userRoleManagerMock.Reset();

            userRoleManagerMock
                .Setup(expression: manager => manager.AddUserRoleAsync(entity: userRole))
                .Throws(exception: exception);

            return controller.Post(newUserRole: userRole);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenDeleteAllFailsAsync()
    {
        // Given
        UserRole[] userRoles = [new() { RoleId = Guid.NewGuid(), UserId = "user-one" }];
        UserRoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userRoleManagerMock.Reset();

            userRoleManagerMock
                .Setup(expression: manager => manager.DeleteAllUserRoleAsync(
                    items: userRoles))
                .Throws(exception: exception);

            return controller.DeleteAll(deletedUserRole: userRoles);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenDeleteFailsAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        UserRoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            userRoleManagerMock.Reset();

            userRoleManagerMock
                .Setup(expression: manager => manager.DeleteUserRoleAsync(
                    entity: It.Is<UserRole>(match: userRole =>
                        userRole.RoleId == roleId
                        && userRole.UserId == "user-one")))
                .Throws(exception: exception);

            return controller.Delete(keyRoleId: roleId, keyUserId: "user-one");
        });
    }

    private UserRoleController CreateController() =>
        new(
            service: userRoleManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object);
}