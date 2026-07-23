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

public partial class UserRoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldReturnProcessingResultWhenSaveAsync()
    {
        // Given
        UserRole entity = CreateRandomUserRole();

        userRoleProcessingServiceMock.Setup(expression: x => x.SaveUserRoleAsync(entity: entity))
            .ReturnsAsync(value: entity);

        // When
        UserRole result = await orchestrationService.SaveUserRoleAsync(
            entity: entity);

        // Then
        result.Should()
            .BeSameAs(expected: entity);

        userRoleProcessingServiceMock.Verify(expression: x => x.SaveUserRoleAsync(entity: entity), times: Times.Once);
        userRoleProcessingServiceMock.VerifyNoOtherCalls();
        userRoleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}