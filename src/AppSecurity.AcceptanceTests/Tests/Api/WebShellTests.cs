// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Web.AcceptanceTests.Infrastructure;
using Xunit;

namespace Web.AcceptanceTests.Tests.Api;

[Collection(WebAcceptanceCollection.Name)]
public sealed partial class WebShellTests(WebAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private Task<HttpResponseMessage> GetRootAsync() =>
        Client.GetAsync(requestUri: "/");

    private Task<string> GetToolsAsync() =>
        Client.GetStringAsync(requestUri: "/tools/index.html");

    private Task<string> GetScriptAsync(string scriptName) =>
        Client.GetStringAsync(requestUri: $"/tools/{scriptName}");
}