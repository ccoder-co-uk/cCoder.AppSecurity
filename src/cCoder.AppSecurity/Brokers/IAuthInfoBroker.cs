// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Brokers;

internal interface IAuthInfoBroker
{
    string GetSSOUserId();
}