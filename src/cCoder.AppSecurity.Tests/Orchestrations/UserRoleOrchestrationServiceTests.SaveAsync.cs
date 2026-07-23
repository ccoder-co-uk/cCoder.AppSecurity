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
        UserRole entity = CreateRandomUserRole();
        userRoleProcessingServiceMock.Setup(x => x.SaveUserRoleAsync(entity)).ReturnsAsync(entity);

        UserRole result = await orchestrationService.SaveUserRoleAsync(entity);

        result.Should().BeSameAs(entity);
        userRoleProcessingServiceMock.Verify(x => x.SaveUserRoleAsync(entity), Times.Once);
        userRoleProcessingServiceMock.VerifyNoOtherCalls();
        userRoleEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}