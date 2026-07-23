// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Brokers.Metadata;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AppSecurityMetadataTypeService : IAppSecurityMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
        TryCatch(operation: IEnumerable<MetadataContainerSet> () =>
        {
            ValidateGetKnownMetadata();

            return [
        new MetadataContainerSet
        {
            Name = "AppSecurity",
            UriBase = "AppSecurity",
            Types =
            [
                Entity<Privilege>(),
                Entity<Role>(),
                Entity<User>(),
                Entity<UserRole>(),
            ],
        },
    ];
        });

    private static ExtendedMetadataContainer Entity<T>()
    {
        ExtendedMetadataContainer metadata = MetadataBroker.CreateExtendedMetadataContainer(
            type: typeof(T),
            isEntity: true,
            hasEndpoint: true);

        metadata.Category = "AppSecurity";

        return metadata;
    }
}