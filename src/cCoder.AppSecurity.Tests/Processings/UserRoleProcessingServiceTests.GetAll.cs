// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class UserRoleProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        UserRole[] links = [new() { UserId = "target-user", RoleId = Guid.NewGuid() }];
        IQueryable<UserRole> queryableLinks = links.AsQueryable();
        userRoleServiceMock.Setup(expression: x => x.GetAll()).Returns(value: queryableLinks);

        // When
        IQueryable<UserRole> result = userRoleProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(expected: queryableLinks);
        userRoleServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        userRoleServiceMock.VerifyNoOtherCalls();
    }

}