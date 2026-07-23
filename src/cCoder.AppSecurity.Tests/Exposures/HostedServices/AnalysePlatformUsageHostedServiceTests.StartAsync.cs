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

        analysePlatformUsageProcessingServiceMock
            .Setup(expression: service => service.RunAsync(cancellationToken: It.IsAny<CancellationToken>()))
            .Callback(action: () => runCompleted.TrySetResult())
            .Returns(value: Task.CompletedTask);

        AnalysePlatformUsageHostedService service = CreateService();

        try
        {
            // When
            await service.StartAsync(cancellationToken: CancellationToken.None);
            await Task.WhenAny(task1: runCompleted.Task, task2: Task.Delay(delay: TimeSpan.FromSeconds(2)));

            // Then
            Assert.True(condition: runCompleted.Task.IsCompleted);
            analysePlatformUsageProcessingServiceMock.Verify(
expression:                 service => service.RunAsync(cancellationToken: It.IsAny<CancellationToken>()),
times:                 Times.Once);
        }
        finally
        {
            await service.StopAsync(cancellationToken: CancellationToken.None);
        }
    }
}