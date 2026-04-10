using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class PrivilegeOrchestrationServiceTests
{
    [Fact]
    public void ShouldReturnProcessingResultWhenGet()
    {
        // Given
        string id = "privilege";
        Privilege entity = CreateRandomPrivilege();
        privilegeProcessingServiceMock.Setup(x => x.Get(id)).Returns(entity);

        // When
        Privilege result = orchestrationService.Get(id);

        // Then
        result.Should().BeSameAs(entity);
        privilegeProcessingServiceMock.Verify(x => x.Get(id), Times.Once);
        privilegeProcessingServiceMock.VerifyNoOtherCalls();
        privilegeEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







