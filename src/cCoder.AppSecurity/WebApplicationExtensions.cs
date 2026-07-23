// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Exposures.EventHandlers;
using cCoder.Data.Exposures;


namespace cCoder.AppSecurity;

public static partial class WebApplicationExtensions
{
    private const string MetadataScope = "AppSecurity";

    public static WebApplication StartAppSecurityWeb(
        this WebApplication app,
        ILogger log = null) =>
        app.UseAppSecurityExposure(log: log)
            .UseAppSecurityEventHandlers()
            .UseAppSecurityDeleteEventHandlers();

    public static WebApplication StartAppSecurityHostedServices(this WebApplication app) =>
        app.UseAppSecurityEventHandlers()
            .UseAppSecurityDeleteEventHandlers();

    private static WebApplication UseAppSecurityExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation(message: "Initialising App Security");
        PopulateMetadataTypeCache(app: app);
        return app;
    }

    private static WebApplication UseAppSecurityEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IAppSecurityEventHandlers handlers in services.GetServices<IAppSecurityEventHandlers>())
        {
            handlers.ListenToAppCreateAndUpdateEvents();
            handlers.ListenToSecurityAccountEvents();
        }

        return app;
    }

    private static WebApplication UseAppSecurityDeleteEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IAppSecurityEventHandlers handlers in services.GetServices<IAppSecurityEventHandlers>())
        {
            handlers.ListenToAppDeleteEvents();
        }

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
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}