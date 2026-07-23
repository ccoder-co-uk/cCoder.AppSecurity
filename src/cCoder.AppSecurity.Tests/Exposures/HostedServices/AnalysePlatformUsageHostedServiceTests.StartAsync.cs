// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Exposures.HostedServices;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures.HostedServices;

public sealed partial class AnalysePlatformUsageHostedServiceTests
{
    [Fact]
    public async Task StartAsync_DelegatesToOrchestrationService()
    {
        // Given
        TaskCompletionSource runCompleted = new();

        analysePlatformUsageOrchestrationServiceMock
            .Setup(service => service.RunAsync(It.IsAny<CancellationToken>()))
            .Callback(() => runCompleted.TrySetResult())
            .Returns(Task.CompletedTask);

        AnalysePlatformUsageHostedService service = CreateService();

        try
        {
            // When
            await service.StartAsync(CancellationToken.None);
            await Task.WhenAny(runCompleted.Task, Task.Delay(TimeSpan.FromSeconds(2)));

            // Then
            Assert.True(runCompleted.Task.IsCompleted);
            analysePlatformUsageOrchestrationServiceMock.Verify(
                service => service.RunAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
        finally
        {
            await service.StopAsync(CancellationToken.None);
        }
    }
}