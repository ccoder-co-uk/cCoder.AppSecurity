// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityOrchestrationValidationException(Exception innerException)
    : Exception(message: "AppSecurity orchestration validation failed.", innerException: innerException);