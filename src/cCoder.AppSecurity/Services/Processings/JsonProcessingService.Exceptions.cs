// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class JsonProcessingService
{
    private static TResult TryCatch<TResult>(Func<TResult> operation)
    {
        try
        {
            return operation();
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