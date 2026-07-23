// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        roleProcessingService = new RoleProcessingService(service: roleServiceMock.Object);
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
            .With(func: x => x.Id = Guid.NewGuid())
            .With(func: x => x.AppId = 1)
            .With(func: x => x.Name = $"Role-{Guid.NewGuid():N}")
            .With(func: x => x.Privs = "app_read")
            .With(func: x => x.App = null)
            .With(func: x => x.Users = [])
            .With(func: x => x.Pages = [])
            .With(func: x => x.Folders = [])
            .Build();
}