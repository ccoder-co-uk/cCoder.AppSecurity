// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class JsonProcessingService(
    IJsonService jsonService)
    : IJsonProcessingService
{
    public T ParseJson<T>(string json) =>
        TryCatch(operation: () =>
        {
            ValidateJsonOnParse(
                json: json);

            return jsonService.ParseJson<T>(
                json: json);
        });

    public string Serialize(object value) =>
        TryCatch(operation: () =>
        {
            ValidateValueOnSerialize(
                value: value);

            return jsonService.Serialize(
                value: value);
        });
}