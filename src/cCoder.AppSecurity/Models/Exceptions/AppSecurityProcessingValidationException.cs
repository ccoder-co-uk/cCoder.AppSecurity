// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityProcessingValidationException(Exception innerException)
    : Exception(message: "AppSecurity processing validation failed.", innerException: innerException);