// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.EventHandlers;
using cCoder.Data.Exposures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests;

public sealed partial class WebApplicationExtensionsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldNotRegisterPackageEventsOnStartupAsync(
        bool hostedServices)
    {
        // Given
        Mock<IAppSecurityEventHandlers> eventHandlersMock = new(
            behavior: MockBehavior.Strict);

        Mock<IMetadataTypeCache> metadataTypeCacheMock = new(
            behavior: MockBehavior.Strict);

        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        eventHandlersMock
            .Setup(expression: handlers =>
                handlers.ListenToAppCreateAndUpdateEvents());

        eventHandlersMock
            .Setup(expression: handlers => handlers.ListenToAppDeleteEvents());

        eventHandlersMock
            .Setup(expression: handlers => handlers.ListenToSecurityAccountEvents());

        metadataTypeCacheMock
            .Setup(expression: cache => cache.Contains(scope: "AppSecurity"))
            .Returns(value: true);

        _ = builder.Services.AddSingleton<IAppSecurityEventHandlers>(
            implementationInstance: eventHandlersMock.Object);

        _ = builder.Services.AddSingleton<IMetadataTypeCache>(
            implementationInstance: metadataTypeCacheMock.Object);

        await using WebApplication app = builder.Build();

        // When
        _ = hostedServices
            ? app.StartAppSecurityHostedServices()
            : app.StartAppSecurityWeb();

        // Then
        eventHandlersMock.Verify(
            expression: handlers =>
                handlers.ListenToAppCreateAndUpdateEvents(),
            times: Times.Once);

        eventHandlersMock.Verify(
            expression: handlers => handlers.ListenToAppDeleteEvents(),
            times: Times.Once);

        eventHandlersMock.Verify(
            expression: handlers => handlers.ListenToSecurityAccountEvents(),
            times: Times.Once);

        eventHandlersMock.VerifyNoOtherCalls();
        metadataTypeCacheMock.VerifyAll();
    }
}