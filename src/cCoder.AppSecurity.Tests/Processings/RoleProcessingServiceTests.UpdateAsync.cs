// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenUpdateAsync()
    {
        // Given
        Role expected = new()
        {
            Id = Guid.NewGuid(),
            Name = "Administrators",
            Privs = "app_update",
        };

        roleServiceMock.Setup(expression: x => x.UpdateRoleAsync(role: expected))
            .ReturnsAsync(value: expected);

        // When
        Role result = await roleProcessingService.UpdateRoleAsync(updatedRole: expected);

        // Then
        Assert.Same(expected: expected, actual: result);
        roleServiceMock.Verify(expression: x => x.UpdateRoleAsync(role: expected), times: Times.Once);
    }

}