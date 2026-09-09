// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityAuthorizationException(Exception innerException)
    : Exception(message: "App security authorization failed.", innerException: innerException);