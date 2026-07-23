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
            .Setup(x => x.DeleteAsync(id))
            .Returns(ValueTask.CompletedTask);

        // When
        await roleProcessingService.DeleteAllRoleAsync(new[] { entity });

        // Then
        roleServiceMock.Verify(x => x.DeleteAsync(id), Times.Once);
        roleServiceMock.VerifyNoOtherCalls();
    }

}