// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class TokenCleanerService
{
    private static async Task TryCatch(Func<Task> operation)
    {
        try
        {
            await operation();
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