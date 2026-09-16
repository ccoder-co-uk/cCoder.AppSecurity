// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;

namespace cCoder.AppSecurity.Brokers.Metadata;

internal interface IMetadataBroker
{
    ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        bool isEntity,
        bool hasEndpoint);
}