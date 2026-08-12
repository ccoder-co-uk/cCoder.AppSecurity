// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Loggings;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.Controllers;
using cCoder.Data.Models.Security;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using Xunit;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public sealed partial class RoleControllerExceptionTests
{
    private readonly Mock<IRoleManager> roleManagerMock = new();
    private readonly Mock<ILoggingBroker> loggingBrokerMock = new();

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        RoleController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }

    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetFails()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        RoleController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.Get(id: roleId))
                .Throws(exception: exception);

            return controller.Get(key: roleId);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPostFailsAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };
        RoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.AddRoleAsync(entity: role))
                .Throws(exception: exception);

            return controller.Post(newRole: role);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPutFailsAsync()
    {
        // Given
        Role role = new() { Id = Guid.NewGuid() };
        RoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.UpdateRoleAsync(entity: role))
                .Throws(exception: exception);

            return controller.Put(key: role.Id, updatedRole: role);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPatchFailsAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        RoleController controller = CreateController();
        Delta<Role> delta = new();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.Get(id: roleId))
                .Throws(exception: exception);

            return controller.Put(key: roleId, updatedDelta: delta);
        });
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenDeleteFailsAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();
        RoleController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            roleManagerMock.Reset();

            roleManagerMock
                .Setup(expression: manager => manager.DeleteAsync(id: roleId))
                .Throws(exception: exception);

            return controller.Delete(key: roleId);
        });
    }

    private RoleController CreateController() =>
        new(
            service: roleManagerMock.Object,
            loggingBroker: loggingBrokerMock.Object);
}