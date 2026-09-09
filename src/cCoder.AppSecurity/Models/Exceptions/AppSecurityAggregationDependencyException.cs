// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityAggregationDependencyException(Exception innerException)
    : Exception(message: "An AppSecurity aggregation dependency failed.", innerException: innerException);