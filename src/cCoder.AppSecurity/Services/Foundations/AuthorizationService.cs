// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AuthorizationService(
    IAuthorizationBroker authorizationBroker)
    : IAuthorizationService
{
    public User GetCurrentUser() =>
        TryCatch(operation: () =>
        {
            ValidateCurrentUserOnGet();

            return authorizationBroker.GetCurrentUser();
        });
}