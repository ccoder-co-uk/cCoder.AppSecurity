namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IAnalysePlatformUsageOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
