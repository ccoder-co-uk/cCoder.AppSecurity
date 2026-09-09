// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityAggregationServiceException(Exception innerException)
    : Exception(message: "The AppSecurity aggregation service failed.", innerException: innerException);