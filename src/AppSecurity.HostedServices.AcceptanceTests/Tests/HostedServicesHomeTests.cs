// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using AppSecurity.HostedServices.AcceptanceTests.Infrastructure;
using Xunit;

namespace AppSecurity.HostedServices.AcceptanceTests.Tests;

[Collection(HostedServicesAcceptanceCollection.Name)]
public sealed partial class HostedServicesHomeTests(HostedServicesAcceptanceFixture fixture)
{
    private HttpClient Client { get; } = fixture.Client;

    private Task<string> GetRootAsync() =>
        Client.GetStringAsync(requestUri: "/");

    private Task<string> GetHealthAsync() =>
        Client.GetStringAsync(requestUri: "/Health");
}