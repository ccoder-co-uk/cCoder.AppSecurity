// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityProcessingServiceException(Exception innerException)
    : Exception(message: "The AppSecurity processing service failed.", innerException: innerException);