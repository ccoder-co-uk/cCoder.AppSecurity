using System;
using System.Text.Json;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Exposures.EventHandlers;
using cCoder.Data.Exposures;


namespace cCoder.AppSecurity;

public static class WebApplicationExtensions
{
    private const string MetadataScope = "AppSecurity";

    public static WebApplication UseAppSecurityExposure(this WebApplication app, ILogger log = null)
    {
        log?.LogInformation("Initialising App Security");
        PopulateMetadataTypeCache(app);
        return app;
    }

    public static WebApplication UseAppSecurityEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IAppSecurityEventHandlers handlers in services.GetServices<IAppSecurityEventHandlers>())
            handlers.ListenToAppCreateAndUpdateEvents();

        return app;
    }

    public static WebApplication UseAppSecurityDeleteEventHandlers(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        foreach (IAppSecurityEventHandlers handlers in services.GetServices<IAppSecurityEventHandlers>())
            handlers.ListenToAppDeleteEvents();

        return app;
    }

    private static void PopulateMetadataTypeCache(WebApplication app)
    {
        IMetadataTypeCache metadataTypeCache = app.Services.GetRequiredService<IMetadataTypeCache>();

        if (!metadataTypeCache.Contains(MetadataScope))
        {
            metadataTypeCache.Set(
                MetadataScope,
                app.Services
                    .GetRequiredService<IAppSecurityMetadataTypeService>()
                    .GetKnownMetadata()
                    .Select(static metadata => JsonSerializer.Serialize(metadata)));
        }
    }
}

