using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Orchestrations;

public interface IAccountEventOrchestrationService
{
    ValueTask ProcessAsync(SecurityAccountEvent accountEvent);
}
