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
    public void ShouldReturnProcessingResultsWhenGetAll()
    {
        // Given
        IQueryable<User> entities = new[] { CreateRandomUser() }.AsQueryable();
        userProcessingServiceMock.Setup(x => x.GetAll(true)).Returns(entities);

        // When
        IQueryable<User> result = orchestrationService.GetAll(true);

        // Then
        result.Should().BeSameAs(entities);
        userProcessingServiceMock.Verify(x => x.GetAll(true), Times.Once);
        userProcessingServiceMock.VerifyNoOtherCalls();
        userEventProcessingServiceMock.VerifyNoOtherCalls();
    }

}







