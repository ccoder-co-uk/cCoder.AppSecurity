namespace cCoder.AppSecurity.Services.Orchestrations;

public interface ITokenCleanerOrchestrationService
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
