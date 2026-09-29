// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityValidationException(Exception innerException)
    : Exception(message: "AppSecurity validation failed.", innerException: innerException);