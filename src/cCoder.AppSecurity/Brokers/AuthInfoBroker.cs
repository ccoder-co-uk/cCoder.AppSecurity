// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;

namespace cCoder.AppSecurity.Brokers;

internal sealed class AuthInfoBroker(ICoreAuthInfo authInfo) : IAuthInfoBroker
{
    public string GetSSOUserId() =>
        authInfo.SSOUserId;
}