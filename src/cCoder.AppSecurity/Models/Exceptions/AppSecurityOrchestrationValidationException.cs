// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityOrchestrationValidationException(Exception innerException)
    : Exception(message: "AppSecurity orchestration validation failed.", innerException: innerException);