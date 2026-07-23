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
        userServiceMock.Setup(x => x.Get(user.Id)).Returns(user);

        // When
        User result = userProcessingService.Get(user.Id);

        // Then
        result.Should().BeSameAs(user);
        userServiceMock.Verify(x => x.Get(user.Id), Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}