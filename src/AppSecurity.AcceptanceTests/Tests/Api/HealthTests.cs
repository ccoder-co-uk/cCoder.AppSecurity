using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class HealthTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private async Task<string> GetHealthAsync() =>
        await Client.GetStringAsync("Health");
}
