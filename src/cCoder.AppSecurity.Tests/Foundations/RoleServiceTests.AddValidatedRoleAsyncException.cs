// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Foundations;

public partial class RoleServiceTests
{
    [Fact]
    public async Task ShouldWrapArgumentExceptionOnAddValidatedRoleAsync()
    {
        // Given

        Role role = CreateRandomRole();
        ArgumentException dependencyException = new(message: "dependency failure");

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Returns(value: ValueTask.FromException<cCoder.Data.Models.Security.Role>(
                exception: dependencyException));

        // When

        Func<Task> addTask = async () => await roleService.AddValidatedRoleAsync(
            newRole: role);

        // Then

        AppSecurityValidationException actualException =
            await Assert.ThrowsAsync<AppSecurityValidationException>(testCode: addTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldWrapDependencyExceptionOnAddValidatedRoleAsync()
    {
        // Given

        Role role = CreateRandomRole();

        AppSecurityDependencyException dependencyException = new(
            innerException: new Exception(message: "dependency failure"));

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Returns(value: ValueTask.FromException<cCoder.Data.Models.Security.Role>(
                exception: dependencyException));

        // When

        Func<Task> addTask = async () => await roleService.AddValidatedRoleAsync(
            newRole: role);

        // Then

        AppSecurityDependencyException actualException =
            await Assert.ThrowsAsync<AppSecurityDependencyException>(testCode: addTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldWrapUnexpectedExceptionOnAddValidatedRoleAsync()
    {
        // Given

        Role role = CreateRandomRole();
        Exception unexpectedException = new(message: "unexpected failure");

        roleBrokerMock
            .Setup(expression: broker => broker.AddRoleAsync(
                entity: It.IsAny<cCoder.Data.Models.Security.Role>()))
            .Returns(value: ValueTask.FromException<cCoder.Data.Models.Security.Role>(
                exception: unexpectedException));

        // When

        Func<Task> addTask = async () => await roleService.AddValidatedRoleAsync(
            newRole: role);

        // Then

        AppSecurityServiceException actualException =
            await Assert.ThrowsAsync<AppSecurityServiceException>(testCode: addTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: unexpectedException);
    }
}