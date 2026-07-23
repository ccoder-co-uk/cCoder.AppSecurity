// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AccountRoleAssignmentService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
        }
        catch (AppSecurityValidationException innerException)
        {
            throw new AppSecurityValidationException(
                innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(
                innerException: innerException);
        }
    }

    private static async ValueTask<TResult> TryCatch<TResult>(
        Func<ValueTask<TResult>> operation)
    {
        try
        {
            return await operation();
        }
        catch (AppSecurityValidationException innerException)
        {
            throw new AppSecurityValidationException(
                innerException: innerException);
        }
        catch (AppSecurityDependencyException innerException)
        {
            throw new AppSecurityDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityServiceException(
                innerException: innerException);
        }
    }
}