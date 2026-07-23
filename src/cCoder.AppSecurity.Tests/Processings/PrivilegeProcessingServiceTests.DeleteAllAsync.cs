// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeProcessingServiceTests
{
    [Fact]
    public async Task ShouldThrowInvalidOperationExceptionWhenUserCanDeleteForDeleteAllAsync()
    {
        // Given
        Privilege privilege = new()
        {
            Id = "privilege",
            Type = "Security",
            Operation = "Delete",
            Description = "Description",
        };
        currentUser = WithPrivilege("privilege_delete");

        // When
        Func<Task> act = async () =>
            await privilegeProcessingService.DeleteAllAsync(new[] { privilege });

        // Then
        var assertions = await act.Should().ThrowAsync<InvalidOperationException>();
        assertions.Which.Message.Should().Be("Cannot delete privileges");
        privilegeServiceMock.VerifyNoOtherCalls();
    }

}