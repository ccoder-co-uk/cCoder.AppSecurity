using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGet()
    {
        // Given

        // When
        Role expected = new()
        {
            Id = Guid.NewGuid(),
            Name = "Administrators",
            Privs = "app_read",
        };
        roleServiceMock.Setup(x => x.Get(expected.Id)).Returns(expected);
        Role result = roleProcessingService.Get(expected.Id);

        // Then
        Assert.Same(expected, result);
    }

}







