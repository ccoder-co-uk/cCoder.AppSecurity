// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.Events;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings.Events;

public partial class AccountEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldPassThroughToOrchestrationForProcessAsync()
    {
        SecurityAccountEvent accountEvent = new()
        {
            RequestDomain = "https://example.com",
            User = new SSOUser
            {
                Id = "new.user",
                Email = "new.user@example.com"
            }
        };

        accountEventOrchestrationServiceMock
            .Setup(service => service.ProcessSecurityAccountEventAsync(accountEvent))
            .Returns(ValueTask.CompletedTask);

        await accountEventProcessingService.ProcessSecurityAccountEventAsync(accountEvent);

        accountEventOrchestrationServiceMock.Verify(
            service => service.ProcessSecurityAccountEventAsync(accountEvent),
            Times.Once);
    }
}