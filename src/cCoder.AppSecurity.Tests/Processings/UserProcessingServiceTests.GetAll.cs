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

public partial class UserProcessingServiceTests
{
    [Fact]
    public void ShouldDelegateToFoundationServiceWhenGetAll()
    {
        // Given
        User[] users = [CreateRandomUser()];
        IQueryable<User> queryableUsers = users.AsQueryable();
        userServiceMock.Setup(expression: x => x.GetAll()).Returns(value: queryableUsers);

        // When
        IQueryable<User> result = userProcessingService.GetAll();

        // Then
        result.Should().BeSameAs(expected: queryableUsers);
        userServiceMock.Verify(expression: x => x.GetAll(), times: Times.Once);
        userServiceMock.VerifyNoOtherCalls();
        coreAuthInfoMock.VerifyNoOtherCalls();
    }

}