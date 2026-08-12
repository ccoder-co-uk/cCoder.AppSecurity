// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Aggregations;

public sealed partial class AppRelationshipAggregationServiceAdditionalTests
{
    private readonly Mock<IAppOrchestrationService> appServiceMock = new();
    private readonly Mock<IPageRoleOrchestrationService> pageRoleServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        AppSecurityMigrationAggregationServiceExceptionTests.ExceptionMappings;

    [Fact]
    public async Task ShouldAddAppThenPageRolesWhenAddAppAsync()
    {
        // Given
        App app = new() { Id = 7 };
        MockSequence sequence = new();

        appServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddAppAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        pageRoleServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.AddOrUpdateAppPageRolesAsync(app: app))
            .Returns(value: ValueTask.CompletedTask);

        AppRelationshipAggregationService service = CreateService();

        // When
        await service.AddAppAsync(newApp: app);

        // Then
        appServiceMock.VerifyAll();
        pageRoleServiceMock.VerifyAll();
    }

    [Fact]
    public async Task ShouldDeleteAppWhenDeleteAppAsync()
    {
        // Given
        App app = new() { Id = 7 };

        appServiceMock
            .Setup(expression: service => service.DeleteAsync(appId: app.Id))
            .Returns(value: ValueTask.CompletedTask);

        AppRelationshipAggregationService service = CreateService();

        // When
        await service.DeleteAppAsync(deletedApp: app);

        // Then
        appServiceMock.VerifyAll();
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddAppFailureAsync(Exception exception, Type expectedType)
    {
        // Given
        App app = new() { Id = 7 };

        appServiceMock
            .Setup(expression: service => service.AddAppAsync(app: app))
            .Throws(exception: exception);

        AppRelationshipAggregationService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddAppAsync(newApp: app);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AppRelationshipAggregationService CreateService() =>
        new(
            appOrchestrationService: appServiceMock.Object,
            pageRoleOrchestrationService: pageRoleServiceMock.Object);
}