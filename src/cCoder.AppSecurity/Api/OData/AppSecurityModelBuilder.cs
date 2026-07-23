// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.OData.Edm;


namespace cCoder.AppSecurity.Api.OData;

internal class AppSecurityModelBuilder : ODataModelBuilder
{
    public AppSecurityModelBuilder(Microsoft.OData.ModelBuilder.ODataConventionModelBuilder builder = null)
        : base(builder: builder) { }

    public override ODataModel Build() =>
        new()
        {
            Context = "AppSecurity",
            Description = "App security endpoints for the platform.",
            EDMModel = BuildEdmModel(),
        };

    public void Configure() => ConfigureModel();

    private IEdmModel BuildEdmModel()
    {
        ConfigureModel();
        return Builder.GetEdmModel();
    }

    private void ConfigureModel()
    {
        AddCommonComplextypes();

        Builder.EntityType<App>().Ignore(propertyExpression: app => app.Config);
        _ = AddSet<App, int>();
        _ = AddSet<User, string>();
        _ = AddSet<Role, Guid>();
        _ = AddSet<Privilege, string>();
        _ = AddJoinSet<UserRole, object>(key: i => new { i.UserId, i.RoleId });

        Builder.Namespace = "";
        _ = Builder.EntityType<User>().Collection.Function(name: "Me").ReturnsFromEntitySet<User>(entitySetName: "User");
    }
}