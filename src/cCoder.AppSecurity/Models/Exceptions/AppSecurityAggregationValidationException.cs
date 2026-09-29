// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityAggregationValidationException(Exception innerException)
    : Exception(message: "AppSecurity aggregation validation failed.", innerException: innerException);