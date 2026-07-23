// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PrivilegeProcessingService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }

    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }

    private static async Task<TResult> TryCatch<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(innerException: innerException);
        }
    }
}