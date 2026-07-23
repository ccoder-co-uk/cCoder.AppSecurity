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

        userProcessingServiceMock.Setup(expression: x => x.Get(id: id))
            .Returns(value: entity);

        // When
        User result = orchestrationService.Get(userId: id);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        userProcessingServiceMock.Verify(expression: x => x.Get(id: id), times: Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}