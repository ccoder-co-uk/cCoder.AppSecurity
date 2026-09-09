// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityOrchestrationServiceException(Exception innerException)
    : Exception(message: "The AppSecurity orchestration service failed.", innerException: innerException);