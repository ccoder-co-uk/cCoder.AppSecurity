// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;
using Moq;
using Xunit;

namespace cCoder.Core.Services.Tests.Security.Orchestrations;

public partial class RoleOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldAddAndUpdateRolesWhenImporting()
    {
        // Given
        Role existingRole = new()
        {
            Id = Guid.NewGuid(),
            AppId = 7,
            Name = "Users"
        };

        Role newRole = new()
        {
            Name = "Guests"
        };

        Role updatedRole = new()
        {
            Name = "Users"
        };

        roleProcessingServiceMock.Setup(expression: service =>
                service.GetAll(ignoreFilters: false))
            .Returns(value: new[] { existingRole }.AsQueryable());

        roleProcessingServiceMock.Setup(expression: service =>
                service.AddValidatedRoleAsync(
entity: It.Is<Role>(match: role =>
                        role.AppId == 7
                        && role.Id == Guid.Empty
                        && role.Name == "Guests")))
            .ReturnsAsync(value: newRole);

        roleProcessingServiceMock.Setup(expression: service =>
                service.UpdateValidatedRoleAsync(
entity: It.Is<Role>(match: role =>
                        role.AppId == 7
                        && role.Id == existingRole.Id
                        && role.Name == "Users")))
            .ReturnsAsync(value: updatedRole);

        // When
        await orchestrationService.ImportRoleAsync(
            appId: 7,
            roles: [newRole, updatedRole]);

        // Then
        roleProcessingServiceMock.Verify(expression: service => service.GetAll(ignoreFilters: false), times: Times.Once);
        roleProcessingServiceMock.Verify(expression: service => service.AddValidatedRoleAsync(entity: newRole), times: Times.Once);
        roleProcessingServiceMock.Verify(expression: service => service.UpdateValidatedRoleAsync(entity: updatedRole), times: Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}