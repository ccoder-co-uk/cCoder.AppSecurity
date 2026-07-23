// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeProcessingServiceTests
{
    [Fact]
    public void ShouldReturnAllPrivilegesWhenUserHasReadPrivilegeForGetAll()
    {
        // Given

        // When
        Privilege[] privileges =
        [
            new()
            {
                Id = "privilege_read",
                Type = "Privilege",
                Operation = "Read",
                Description = "Read privileges",
            },
        ];
        privilegeServiceMock.Setup(x => x.GetAll()).Returns(privileges.AsQueryable());
        currentUser = WithPrivilege("privilege_read");
        Privilege[] result = privilegeProcessingService.GetAll().ToArray();

        // Then
        Assert.Single(result);
        Assert.Equal("privilege_read", result[0].Id);
    }

}