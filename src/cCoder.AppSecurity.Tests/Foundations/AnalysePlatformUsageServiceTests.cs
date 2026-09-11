// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Services.Foundations;
using Moq;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class AnalysePlatformUsageServiceTests
{
    private readonly Mock<IJsonBroker> jsonBrokerMock = new();

    private AnalysePlatformUsageService CreateService() =>
        new(jsonBroker: jsonBrokerMock.Object);
}