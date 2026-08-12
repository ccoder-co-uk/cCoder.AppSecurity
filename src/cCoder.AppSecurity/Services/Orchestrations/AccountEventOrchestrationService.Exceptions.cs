// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Orchestrations;

internal sealed partial class AccountEventOrchestrationService
{
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

}