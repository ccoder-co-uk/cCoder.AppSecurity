// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserProcessingServiceTests
{
    private User currentUser = WithoutPrivileges();
    private readonly Mock<IUserService> userServiceMock = new();
    private readonly Mock<ICoreAuthInfo> coreAuthInfoMock = new();
    private readonly UserProcessingService userProcessingService;

    public UserProcessingServiceTests()
    {
        userProcessingService = new UserProcessingService(
            userServiceMock.Object,
            coreAuthInfoMock.Object
        );
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

    private static User CreateRandomUser(string id = "test-user", string email = null) =>
        Builder<User>
            .CreateNew()
            .With(x => x.Id = id)
            .With(x => x.DefaultCultureId = string.Empty)
            .With(x => x.DisplayName = $"User-{Guid.NewGuid():N}")
            .With(x => x.Email = email ?? $"{Guid.NewGuid():N}@example.com")
            .With(x => x.IsActive = true)
            .With(x => x.DefaultCulture = null)
            .With(x => x.Roles = [])
            .Build();
}