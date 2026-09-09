// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityProcessingDependencyException(Exception innerException)
    : Exception(message: "An AppSecurity processing dependency failed.", innerException: innerException);