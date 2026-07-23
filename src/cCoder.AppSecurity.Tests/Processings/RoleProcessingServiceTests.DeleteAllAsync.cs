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
    public async Task ShouldUseFoundationDeleteAsyncPerItemWhenDeleteAllAsync()
    {
        // Given
        Role entity = CreateRandomRole();
        var id = entity.Id;

        roleServiceMock
            .Setup(expression: x => x.DeleteAsync(id: id))
            .Returns(value: ValueTask.CompletedTask);

        // When
        await roleProcessingService.DeleteAllRoleAsync(deletedRole: new[] { entity });

        // Then
        roleServiceMock.Verify(expression: x => x.DeleteAsync(id: id), times: Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
    }

}