// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given

        // When
        Role expected = new()
        {
            Id = Guid.NewGuid(),
            Name = "Administrators",
            Privs = "app_read",
        };
        roleServiceMock.Setup(expression: x => x.Get(id: expected.Id)).Returns(value: expected);
        Role result = roleProcessingService.Get(roleId: expected.Id);

        // Then
        Assert.Same(expected: expected, actual: result);
    }

}