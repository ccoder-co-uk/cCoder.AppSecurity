// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityOrchestrationDependencyException(Exception innerException)
    : Exception(message: "An AppSecurity orchestration dependency failed.", innerException: innerException);