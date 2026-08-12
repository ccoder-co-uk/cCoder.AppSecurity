// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class AccountRoleAssignmentProcessingServiceTests
{
    private readonly Mock<IAccountRoleAssignmentService> assignmentServiceMock = new();

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    [InlineData(true, false)]
    public async Task ShouldOnlyAttachUsersRoleWhenRoleExistsAndIsNotAssigned(
        bool roleExists,
        bool isAssigned)
    {
        // Given
        Guid? roleId = roleExists ? Guid.NewGuid() : null;
        User user = new() { Id = "user-one" };

        AccountRoleAssignment assignment = new()
        {
            AppId = 7,
            UserId = user.Id,
            RoleId = roleId,
            IsAssigned = isAssigned
        };

        assignmentServiceMock
            .Setup(expression: service => service.GetAccountRoleAssignment(
                accountRoleAssignment: It.IsAny<AccountRoleAssignment>()))
            .Returns(value: assignment);

        if (roleExists && !isAssigned)
        {
            assignmentServiceMock
                .Setup(expression: service => service.AddAccountRoleAssignmentAsync(
                    newAccountRoleAssignment: assignment))
                .ReturnsAsync(value: assignment);
        }

        AccountRoleAssignmentProcessingService service = CreateService();

        // When
        await service.AttachUsersRoleAsync(user: user, appId: 7);

        // Then
        assignmentServiceMock.Verify(
            expression: dependency => dependency.AddAccountRoleAssignmentAsync(
                newAccountRoleAssignment: assignment),
            times: roleExists && !isAssigned ? Times.Once() : Times.Never());
    }

    [Fact]
    public async Task ShouldMapValidationFailureWhenAttachUsersRoleAsyncReceivesNullUser()
    {
        // Given
        AccountRoleAssignmentProcessingService service = CreateService();

        // When
        Func<Task> action = async () => await service.AttachUsersRoleAsync(
            user: null,
            appId: 7);

        // Then
        await action
            .Should()
            .ThrowAsync<AppSecurityProcessingValidationException>();
    }

    private AccountRoleAssignmentProcessingService CreateService() =>
        new(accountRoleAssignmentService: assignmentServiceMock.Object);
}