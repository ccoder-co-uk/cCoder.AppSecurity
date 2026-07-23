// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class RoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given

        // When
        IQueryable<Role> expected = new[]
        {
            new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrators",
                Privs = "app_read",
            },
        }.AsQueryable();

        roleServiceMock.Setup(expression: x => x.GetAll())
            .Returns(value: expected);

        IQueryable<Role> result = roleProcessingService.GetAll();

        // Then
        Assert.Single(collection: result);
    }

}