// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Aggregations;

public sealed partial class AppRelationshipAggregationServiceTests
{
    [Fact]
    public async Task ShouldProcessRolesBeforePageRolesOnUpdateAsync()
    {
        // Given
        App app = new() { Id = 7 };
        MockSequence sequence = new();

        Mock<IAppOrchestrationService> appOrchestrationServiceMock =
            new(behavior: MockBehavior.Strict);

        Mock<IPageRoleOrchestrationService> pageRoleOrchestrationServiceMock =
            new(behavior: MockBehavior.Strict);

        appOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.UpdateAppAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        pageRoleOrchestrationServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdateAppPageRolesAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        AppRelationshipAggregationService aggregationService = new(
            appOrchestrationService: appOrchestrationServiceMock.Object,
            pageRoleOrchestrationService: pageRoleOrchestrationServiceMock.Object);

        // When
        await aggregationService.UpdateAppAsync(updatedApp: app);

        // Then
        appOrchestrationServiceMock.VerifyAll();
        pageRoleOrchestrationServiceMock.VerifyAll();
    }
}