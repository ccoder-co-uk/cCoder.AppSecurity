// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class EventHandlerService
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

}