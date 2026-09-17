// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Exposures;


namespace cCoder.AppSecurity;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "AppSecurity";

    public static WebApplication StartAppSecurityWeb(
        this WebApplication app,
        ILogger log = null) =>
        app.UseAppSecurityExposure(log: log);

    public static WebApplication StartAppSecurityHostedServices(
        this WebApplication app)
    {
        PopulateMetadataTypeCache(app: app);

        return app;
    }

    private static WebApplication UseAppSecurityExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising App Security");
        PopulateMetadataTypeCache(app: app);
        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(scope: MetadataScope))
        {
            metadataTypeCache.Set(
scope: MetadataScope,
typeSetPayloads: app.Services
                    .GetRequiredService<IAppSecurityMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(selector: static metadata => JsonSerializer.Serialize(value: metadata)));
        }
    }
}