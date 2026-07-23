// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models.Exceptions;

internal sealed class AppSecurityValidationException(Exception innerException)
    : Exception("AppSecurity validation failed.", innerException);

internal sealed class AppSecurityDependencyException(Exception innerException)
    : Exception("An AppSecurity dependency failed.", innerException);

internal sealed class AppSecurityServiceException(Exception innerException)
    : Exception("The AppSecurity service failed.", innerException);

internal sealed class AppSecurityProcessingValidationException(Exception innerException)
    : Exception("AppSecurity processing validation failed.", innerException);

internal sealed class AppSecurityProcessingDependencyException(Exception innerException)
    : Exception("An AppSecurity processing dependency failed.", innerException);

internal sealed class AppSecurityProcessingServiceException(Exception innerException)
    : Exception("The AppSecurity processing service failed.", innerException);

internal sealed class AppSecurityOrchestrationValidationException(Exception innerException)
    : Exception("AppSecurity orchestration validation failed.", innerException);

internal sealed class AppSecurityOrchestrationDependencyException(Exception innerException)
    : Exception("An AppSecurity orchestration dependency failed.", innerException);

internal sealed class AppSecurityOrchestrationServiceException(Exception innerException)
    : Exception("The AppSecurity orchestration service failed.", innerException);

internal sealed class AppSecurityAggregationValidationException(Exception innerException)
    : Exception("AppSecurity aggregation validation failed.", innerException);

internal sealed class AppSecurityAggregationDependencyException(Exception innerException)
    : Exception("An AppSecurity aggregation dependency failed.", innerException);

internal sealed class AppSecurityAggregationServiceException(Exception innerException)
    : Exception("The AppSecurity aggregation service failed.", innerException);