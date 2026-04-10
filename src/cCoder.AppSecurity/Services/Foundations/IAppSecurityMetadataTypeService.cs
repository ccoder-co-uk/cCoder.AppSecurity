using cCoder.AppSecurity.Api.OData;


namespace cCoder.AppSecurity.Services.Foundations;

internal interface IAppSecurityMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}

