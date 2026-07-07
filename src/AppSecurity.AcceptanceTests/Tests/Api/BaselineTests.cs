using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class BaselineTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;
}
