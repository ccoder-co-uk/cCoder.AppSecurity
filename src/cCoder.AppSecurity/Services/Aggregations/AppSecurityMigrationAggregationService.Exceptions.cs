// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppSecurityMigrationAggregationService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

    private static async Task<TResult> TryCatch<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }
}