// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace cCoder.AppSecurity.Brokers.OData;

internal sealed class AppSecurityODataModelBroker(
    ODataConventionModelBuilder builder = null)
    : IAppSecurityODataModelBroker
{
    private readonly ODataConventionModelBuilder builder =
        builder ?? new ODataConventionModelBuilder();

    public ODataModel SelectODataModel() =>
        new()
        {
            Context = "AppSecurity",
            Description = "App security endpoints for the platform.",
            EDMModel = SelectEdmModel(),
        };

    public void ConfigureODataModel()
    {
        builder.ComplexType<MetadataContainerSet>();
        builder.ComplexType<MetadataContainer>();
        builder.ComplexType<PropertyContainer>();
        builder.ComplexType<AuditResultsByUser>();
        builder.ComplexType<AuditResultByProperty>();

        builder.EntityType<App>()
            .Ignore(propertyExpression: app => app.Config);

        builder.EntitySet<App>(name: nameof(App));
        builder.EntitySet<User>(name: nameof(User));
        builder.EntitySet<Role>(name: nameof(Role));
        builder.EntitySet<Privilege>(name: nameof(Privilege));
        builder.EntitySet<UserRole>(name: nameof(UserRole));

        builder.EntityType<UserRole>()
            .HasKey(keyDefinitionExpression: userRole => new
            {
                userRole.UserId,
                userRole.RoleId,
            });

        builder.Namespace = "";

        builder.EntityType<User>()
            .Collection
            .Function(name: "Me")
            .ReturnsFromEntitySet<User>(entitySetName: nameof(User));
    }

    private IEdmModel SelectEdmModel()
    {
        ConfigureODataModel();

        return builder.GetEdmModel();
    }
}