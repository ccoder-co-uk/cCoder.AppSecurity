// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Dependencies;
using cCoder.Data.Models.CMS;

namespace cCoder.AppSecurity.Services.Aggregations;

internal sealed partial class AppRelationshipAggregationService
{
    private static void ValidateAppOnAdd(App app) =>
        ValidationRulesEngine.Validate(inputs: [app]);

    private static void ValidateAppOnUpdate(App app) =>
        ValidationRulesEngine.Validate(inputs: [app]);

    private static void ValidateAppOnDelete(App app) =>
        ValidationRulesEngine.Validate(inputs: [app]);
}