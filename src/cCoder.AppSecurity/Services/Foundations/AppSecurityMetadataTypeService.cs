using cCoder.AppSecurity.Api.OData;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

internal sealed class AppSecurityMetadataTypeService : IAppSecurityMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
    [
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

    private static ExtendedMetadataContainer Entity<T>() =>
        new(typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "AppSecurity",
        };
}

