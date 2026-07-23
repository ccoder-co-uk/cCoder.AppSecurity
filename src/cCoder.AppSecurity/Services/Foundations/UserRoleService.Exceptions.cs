// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class UserRoleService
{
    private static void TryCatch(Action operation)
    {
        try
        {
            operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }

    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }

    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }

    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }

    private static async Task<TResult> TryCatch<TResult>(Func<Task<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (ArgumentException innerException)
        {
            throw new AppSecurityValidationException(innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(innerException: innerException);
        }
    }
}