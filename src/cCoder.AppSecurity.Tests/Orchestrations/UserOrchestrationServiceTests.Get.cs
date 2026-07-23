// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class UserOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        string id = "user-id";
        User entity = CreateRandomUser();
        userProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        User result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        userProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}