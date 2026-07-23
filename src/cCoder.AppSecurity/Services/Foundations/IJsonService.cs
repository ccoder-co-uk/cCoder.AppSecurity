// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IJsonService
{
    T ParseJson<T>(string json);
    string Serialize(object value);
}