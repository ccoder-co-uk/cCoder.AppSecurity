// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityDependencyException(Exception innerException)
    : Exception(message: "An AppSecurity dependency failed.", innerException: innerException);