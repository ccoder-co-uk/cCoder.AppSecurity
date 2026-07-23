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

public partial class PrivilegeProcessingServiceTests
{
    [Fact]
    public async Task ShouldUseReadsAndReturnFailedAddResultWhenPrivilegeIsNewForAddOrUpdate()
    {
        // Given
        authorizationBrokerMock.Setup(x => x.GetCurrentUser()).Returns(() => ToExternalUser(currentUser));
        Privilege privilege = new()
        {
            Id = string.Empty,
            Type = "Security",
            Operation = "Create",
            Description = "Description",
        };
        User actor = WithPrivileges(new[] { "privilege_create" });
        currentUser = actor;

        // When
        Result<Privilege>[] results = (
            await privilegeProcessingService.AddOrUpdate(new[] { privilege })
        ).ToArray();

        // Then
        results.Should().ContainSingle();
        results[0].Success.Should().BeFalse();
        results[0].Item.Should().BeSameAs(privilege);
        results[0].Message.Should().Be("Cannot add privileges");
        privilegeServiceMock.VerifyNoOtherCalls();
    }

}