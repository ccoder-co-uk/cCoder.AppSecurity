// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public async Task ShouldDelegateToFoundationServiceWhenDeleteAsync()
    {
        // Given
        Guid roleId = Guid.NewGuid();

        roleServiceMock.Setup(expression: x => x.DeleteAsync(id: roleId))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await roleProcessingService.DeleteAsync(roleId: roleId);

        // Then
        roleServiceMock.Verify(expression: x => x.DeleteAsync(id: roleId), times: Times.Once);
    }

}