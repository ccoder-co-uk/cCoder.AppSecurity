// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public void ShouldWrapArgumentExceptionOnGetAll()
    {
        // Given

        ArgumentException dependencyException = new(message: "dependency failure");

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: false))
            .Throws(exception: dependencyException);

        // When

        Action getAllAction = () => roleService.GetAll(ignoreFilters: false);

        // Then

        AppSecurityValidationException actualException =
            Assert.Throws<AppSecurityValidationException>(testCode: getAllAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapDependencyExceptionOnGetAll()
    {
        // Given

        AppSecurityDependencyException dependencyException = new(
            innerException: new Exception(message: "dependency failure"));

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: false))
            .Throws(exception: dependencyException);

        // When

        Action getAllAction = () => roleService.GetAll(ignoreFilters: false);

        // Then

        AppSecurityDependencyException actualException =
            Assert.Throws<AppSecurityDependencyException>(testCode: getAllAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapUnexpectedExceptionOnGetAll()
    {
        // Given

        Exception unexpectedException = new(message: "unexpected failure");

        roleBrokerMock
            .Setup(expression: broker => broker.GetAllRoles(ignoreFilters: false))
            .Throws(exception: unexpectedException);

        // When

        Action getAllAction = () => roleService.GetAll(ignoreFilters: false);

        // Then

        AppSecurityServiceException actualException =
            Assert.Throws<AppSecurityServiceException>(testCode: getAllAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: unexpectedException);
    }
}