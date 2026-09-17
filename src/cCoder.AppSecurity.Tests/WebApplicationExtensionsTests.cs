// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Exposures;
using cCoder.Eventing;
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
    public async Task AppSecurityStartup_WhenInvoked_DoesNotRegisterEventListenersAsync(
        bool hostedServices)
    {
        // Given
        Mock<IEventHub> eventHubMock = new(
            behavior: MockBehavior.Strict);

        Mock<IMetadataTypeCache> metadataTypeCacheMock = new(
            behavior: MockBehavior.Strict);

        WebApplicationBuilder builder = WebApplication.CreateBuilder();

        metadataTypeCacheMock
            .Setup(expression: cache => cache.Contains(scope: "AppSecurity"))
            .Returns(value: true);

        _ = builder.Services.AddSingleton<IEventHub>(
            implementationInstance: eventHubMock.Object);

        _ = builder.Services.AddSingleton<IMetadataTypeCache>(
            implementationInstance: metadataTypeCacheMock.Object);

        await using WebApplication app = builder.Build();

        // When
        _ = hostedServices
            ? app.StartAppSecurityHostedServices()
            : app.StartAppSecurityWeb();

        // Then
        eventHubMock.VerifyNoOtherCalls();
        metadataTypeCacheMock.VerifyAll();
    }
}