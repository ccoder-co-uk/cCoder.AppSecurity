// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Processings;

internal interface IJsonProcessingService
{
    T ParseJson<T>(string json);
    string Serialize(object value);
}