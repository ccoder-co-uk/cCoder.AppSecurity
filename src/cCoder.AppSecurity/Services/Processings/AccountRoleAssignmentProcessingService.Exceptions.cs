// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class AccountRoleAssignmentProcessingService
{
    private static async ValueTask TryCatch(
        Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityProcessingValidationException innerException)
        {
            throw new AppSecurityProcessingValidationException(
                innerException: innerException);
        }
        catch (AppSecurityProcessingDependencyException innerException)
        {
            throw new AppSecurityProcessingDependencyException(
                innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityProcessingServiceException(
                innerException: innerException);
        }
    }
}