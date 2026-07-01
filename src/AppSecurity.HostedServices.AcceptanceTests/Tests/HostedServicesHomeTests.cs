using AppSecurity.HostedServices.AcceptanceTests.Infrastructure;
using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Tests;

[Collection(HostedServicesAcceptanceCollection.Name)]
public sealed partial class HostedServicesHomeTests(HostedServicesAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private Task<string> GetRootAsync() =>
        Client.GetStringAsync("/");

    private Task<string> GetHealthAsync() =>
        Client.GetStringAsync("/Health");
}
