// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class JsonService(
    IJsonBroker jsonBroker)
    : IJsonService
{
    public T ParseJson<T>(string json) =>
        TryCatch(operation: () =>
        {
            ValidateJsonOnParse(
                json: json);

            return jsonBroker.ParseJson<T>(
                json: json);
        });

    public string Serialize(object value) =>
        TryCatch(operation: () =>
        {
            ValidateValueOnSerialize(
                value: value);

            return jsonBroker.Serialize(
                value: value);
        });
}