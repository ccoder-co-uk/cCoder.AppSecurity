// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;
using Microsoft.AspNetCore.OData.Deltas;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public async Task ShouldReturnNotFoundWhenPatchCannotFindPrivilegeAsync()
    {
        // Given
#pragma warning disable STXFORMAT008
        privilegeManagerMock
            .Setup(expression: manager => manager.Get(id: "missing"))
            .Returns(value: null);
#pragma warning restore STXFORMAT008

        PrivilegeController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: "missing",
            updatedDelta: new Delta<Privilege>());

        // Then
        result
            .Should()
            .BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ShouldApplyDeltaAndUpdatePrivilegeWhenPatchSucceedsAsync()
    {
        // Given
        Privilege privilege = new() { Id = "page_read", Description = "Old" };
        Delta<Privilege> delta = new();
        delta.TrySetPropertyValue(name: nameof(Privilege.Description), value: "New");

        privilegeManagerMock
            .Setup(expression: manager => manager.Get(id: privilege.Id))
            .Returns(value: privilege);

#pragma warning disable STXFORMAT008
        privilegeManagerMock
            .Setup(expression: manager => manager.UpdatePrivilegeAsync(
                entity: privilege))
            .ReturnsAsync(value: privilege);
#pragma warning restore STXFORMAT008

        PrivilegeController controller = CreateController();

        // When
        IActionResult result = await controller.Put(
            key: privilege.Id,
            updatedDelta: delta);

        // Then
        result
            .Should()
            .BeOfType<OkObjectResult>();

        privilege.Description
            .Should()
            .Be(expected: "New");
    }

    [Fact]
    public async Task ShouldReturnExpectedStatusCodesWhenPatchFailsAsync()
    {
        // Given
        Delta<Privilege> delta = new();
        PrivilegeController controller = CreateController();

        // When

        // Then
        await AssertExceptionStatusCodesAsync(invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.Get(id: "page_read"))
                .Throws(exception: exception);

            return controller.Put(key: "page_read", updatedDelta: delta);
        });
    }
}