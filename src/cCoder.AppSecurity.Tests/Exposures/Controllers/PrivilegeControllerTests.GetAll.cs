// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using cCoder.AppSecurity.Exposures.Controllers;
using static cCoder.AppSecurity.Tests.Exposures.Controllers.PrivilegeControllerTestSupport;

namespace cCoder.AppSecurity.Tests.Exposures.Controllers;

public partial class PrivilegeControllerTests
{
    [Fact]
    public void ShouldReturnExpectedStatusCodesWhenGetAllFails()
    {
        // Given
        PrivilegeController controller = CreateController();

        // When

        // Then
        AssertExceptionStatusCodes(invoke: exception =>
        {
            privilegeManagerMock.Reset();

            privilegeManagerMock
                .Setup(expression: manager => manager.GetAll(ignoreFilters: false))
                .Throws(exception: exception);

            return controller.GetAll();
        });
    }
}