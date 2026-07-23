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

        roleProcessingServiceMock.Setup(service =>
                service.GetAll(false))
            .Returns(new[] { existingRole }.AsQueryable());

        roleProcessingServiceMock.Setup(service =>
                service.AddValidatedRoleAsync(
                    It.Is<Role>(role =>
                        role.AppId == 7
                        && role.Id == Guid.Empty
                        && role.Name == "Guests")))
            .ReturnsAsync(newRole);

        roleProcessingServiceMock.Setup(service =>
                service.UpdateValidatedRoleAsync(
                    It.Is<Role>(role =>
                        role.AppId == 7
                        && role.Id == existingRole.Id
                        && role.Name == "Users")))
            .ReturnsAsync(updatedRole);

        await orchestrationService.ImportRoleAsync(7, [newRole, updatedRole]);

        roleProcessingServiceMock.Verify(service => service.GetAll(false), Times.Once);
        roleProcessingServiceMock.Verify(service => service.AddValidatedRoleAsync(newRole), Times.Once);
        roleProcessingServiceMock.Verify(service => service.UpdateValidatedRoleAsync(updatedRole), Times.Once);
        roleProcessingServiceMock.VerifyNoOtherCalls();
        roleEventProcessingServiceMock.VerifyNoOtherCalls();
    }
}