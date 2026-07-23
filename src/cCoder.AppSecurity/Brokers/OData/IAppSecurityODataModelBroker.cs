// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;

namespace cCoder.AppSecurity.Brokers.OData;

internal interface IAppSecurityODataModelBroker
{
    ODataModel SelectODataModel();
    void ConfigureODataModel();
}