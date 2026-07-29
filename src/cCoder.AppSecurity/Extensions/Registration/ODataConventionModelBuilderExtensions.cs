// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.OData;
using Microsoft.OData.ModelBuilder;

namespace cCoder.AppSecurity;

internal static class ODataConventionModelBuilderExtensions
{
    internal static void ConfigureAppSecurityApiModel(
        this ODataConventionModelBuilder builder) =>
        new AppSecurityODataModelBroker(
            builder: builder).ConfigureODataModel();
}