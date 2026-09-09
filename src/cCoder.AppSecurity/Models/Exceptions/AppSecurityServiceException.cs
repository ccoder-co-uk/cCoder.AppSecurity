// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityServiceException(Exception innerException)
    : Exception(message: "The AppSecurity service failed.", innerException: innerException);