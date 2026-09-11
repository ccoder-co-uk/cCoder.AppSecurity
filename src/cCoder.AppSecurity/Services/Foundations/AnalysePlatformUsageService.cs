// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AnalysePlatformUsageService(
    IJsonBroker jsonBroker)
    : IAnalysePlatformUsageService
{
    public string Serialize(object value) =>
        TryCatch(operation: () =>
        {
            ValidateValueOnSerialize(value: value);

            return jsonBroker.Serialize(value: value);
        });
}