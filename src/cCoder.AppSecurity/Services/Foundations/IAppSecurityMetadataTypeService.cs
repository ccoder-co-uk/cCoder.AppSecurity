// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;

using cCoder.AppSecurity.Api.OData;


namespace cCoder.AppSecurity.Services.Foundations;

internal interface IAppSecurityMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}