using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    private User currentUser = WithoutPrivileges();
    private readonly Mock<IRoleService> roleServiceMock = new();
    private readonly RoleProcessingService roleProcessingService;

    public RoleProcessingServiceTests()
    {
        roleProcessingService = new RoleProcessingService(roleServiceMock.Object);
    }

    private static User WithoutPrivileges() =>
        new()
        {
            Id = "test-user",
            DefaultCultureId = string.Empty,
            DisplayName = "Test User",
            Email = "test@example.com",
            IsActive = true,
            Roles = [],
        };

    private static Role CreateRandomRole() =>
        Builder<Role>
            .CreateNew()
            .With(x => x.Id = Guid.NewGuid())
            .With(x => x.AppId = 1)
            .With(x => x.Name = $"Role-{Guid.NewGuid():N}")
            .With(x => x.Privs = "app_read")
            .With(x => x.App = null)
            .With(x => x.Users = [])
            .With(x => x.Pages = [])
            .With(x => x.Folders = [])
            .Build();
}









