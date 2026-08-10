// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Models.Exceptions;

namespace cCoder.AppSecurity.Services.Processings;

internal sealed partial class PageRoleProcessingService
{
    private static void ValidatePageRolesOnAddOrUpdate(IEnumerable<Role> roles) =>
        ValidationRulesEngine.Validate(inputs: [roles]);

    private static void ValidatePageRoleDependenciesOnAddOrUpdate(
        Guid roleId,
        int pageId)
    {
        ValidationRulesEngine.Validate(inputs: [roleId]);

        if (pageId == 0)
        {
            throw new AppSecurityProcessingValidationException(
                innerException: new ArgumentException(
                    message: "Page role dependencies could not be resolved."));
        }
    }
}