// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AppSecurityPackageOrchestrationService
{
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
            when (innerException.GetBaseException() is System.Security.SecurityException)
        {
            throw new AppSecurityAuthorizationException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityOrchestrationServiceException(innerException: innerException);
        }
    }

}