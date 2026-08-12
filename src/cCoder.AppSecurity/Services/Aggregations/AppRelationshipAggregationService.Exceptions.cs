// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppRelationshipAggregationService
{
    private static async ValueTask TryCatch(Func<ValueTask> operation)
    {
        try
        {
            await operation();
        }
        catch (AppSecurityAggregationValidationException innerException)
        {
            throw new AppSecurityAggregationValidationException(innerException: innerException);
        }
        catch (AppSecurityAggregationDependencyException innerException)
        {
            throw new AppSecurityAggregationDependencyException(innerException: innerException);
        }
        catch (Exception innerException)
        {
            throw new AppSecurityAggregationServiceException(innerException: innerException);
        }
    }

}