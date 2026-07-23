// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using AppSecurityOrchestrationDependencyException =
    cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingDependencyException;
using AppSecurityOrchestrationServiceException =
    cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingServiceException;
using AppSecurityOrchestrationValidationException =
    cCoder.AppSecurity.Models.Exceptions.AppSecurityProcessingValidationException;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AnalysePlatformUsageProcessingService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

    private static async Task<TResult> TryCatch<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityOrchestrationValidationException innerException)
        {
            throw new AppSecurityOrchestrationValidationException(innerException: innerException);
        }
        catch (AppSecurityOrchestrationDependencyException innerException)
        {
            throw new AppSecurityOrchestrationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }
}