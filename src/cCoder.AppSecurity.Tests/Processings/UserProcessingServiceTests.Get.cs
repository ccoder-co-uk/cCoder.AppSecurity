// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given
        User user = CreateRandomUser();
        userServiceMock.Setup(expression: x => x.Get(id: user.Id)).Returns(value: user);

        // When
        User result = userProcessingService.Get(userId: user.Id);

        // Then
        result.Should().BeSameAs(expected: user);
        userServiceMock.Verify(expression: x => x.Get(id: user.Id), times: Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}