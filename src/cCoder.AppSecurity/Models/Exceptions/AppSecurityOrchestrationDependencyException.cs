// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityOrchestrationDependencyException(Exception innerException)
    : Exception(message: "An AppSecurity orchestration dependency failed.", innerException: innerException);