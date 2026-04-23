using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.EventHandlers;
using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using cCoder.Eventing;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.AppSecurity.Brokers.AuthorizationBroker;
using EventHubBroker = cCoder.AppSecurity.Brokers.Events.EventHubBroker;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IEventHubBroker = cCoder.AppSecurity.Brokers.Events.IEventHubBroker;
using IJsonBroker = cCoder.AppSecurity.Brokers.IJsonBroker;
using IPrivilegeBroker = cCoder.AppSecurity.Brokers.IPrivilegeBroker;
using IPrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.IPrivilegeEventBroker;
using IRoleBroker = cCoder.AppSecurity.Brokers.IRoleBroker;
using IRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IRoleEventBroker;
using IUserBroker = cCoder.AppSecurity.Brokers.Storages.IUserBroker;
using IUserEventBroker = cCoder.AppSecurity.Brokers.Events.IUserEventBroker;
using IUserRoleBroker = cCoder.AppSecurity.Brokers.Storages.IUserRoleBroker;
using IUserRoleEventBroker = cCoder.AppSecurity.Brokers.Events.IUserRoleEventBroker;
using JsonBroker = cCoder.AppSecurity.Brokers.JsonBroker;
using PrivilegeBroker = cCoder.AppSecurity.Brokers.PrivilegeBroker;
using PrivilegeEventBroker = cCoder.AppSecurity.Brokers.Events.PrivilegeEventBroker;
using RoleBroker = cCoder.AppSecurity.Brokers.RoleBroker;
using RoleEventBroker = cCoder.AppSecurity.Brokers.Events.RoleEventBroker;
using UserBroker = cCoder.AppSecurity.Brokers.Storages.UserBroker;
using UserEventBroker = cCoder.AppSecurity.Brokers.Events.UserEventBroker;
using UserRoleBroker = cCoder.AppSecurity.Brokers.Storages.UserRoleBroker;
using UserRoleEventBroker = cCoder.AppSecurity.Brokers.Events.UserRoleEventBroker;


namespace cCoder.AppSecurity;

public static class IServiceCollectionExtensions
{
    public static void AddAppSecurity(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddAggregations();
        services.AddEventHandlers();
    }

    public static void AddAppSecurityApi(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddAppSecurity();
        services.AddApi("AppSecurity", ConfigureAppSecurityApiModel, builder);
    }

    public static void AddAppSecurityHostedServices(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddAggregations();
        services.AddTransient<IAnalysePlatformUsageOrchestrationService, AnalysePlatformUsageOrchestrationService>();
        services.AddTransient<ITokenCleanerOrchestrationService, TokenCleanerOrchestrationService>();
        services.AddHostedService<AnalysePlatformUsageHostedService>();
        services.AddHostedService<TokenCleanerHostedService>();
    }

    private static void AddEventingTypes(this IServiceCollection services)
    {
        services.AddEventingForType<App>();
        services.AddEventingForType<Privilege>();
        services.AddEventingForType<Package>();
        services.AddEventingForType<PackageItem>();
        services.AddEventingForType<(int, Package)>();
        services.AddEventingForType<Role>();
        services.AddEventingForType<User>();
        services.AddEventingForType<UserRole>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IEventHubBroker, EventHubBroker>();
        services.AddTransient<IJsonBroker, JsonBroker>();
        services.AddTransient<IRoleEventBroker, RoleEventBroker>();
        services.AddTransient<IPrivilegeEventBroker, PrivilegeEventBroker>();
        services.AddTransient<IUserEventBroker, UserEventBroker>();
        services.AddTransient<IUserRoleEventBroker, UserRoleEventBroker>();
        services.AddTransient<IRoleBroker, RoleBroker>();
        services.AddTransient<IPrivilegeBroker, PrivilegeBroker>();
        services.AddTransient<IUserBroker, UserBroker>();
        services.AddTransient<IUserRoleBroker, UserRoleBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IAppSecurityAppExposure, AppSecurityAppExposure>();
        services.AddTransient<IAppSecurityPackageManager, AppSecurityPackageManager>();
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IPrivilegeEventService, PrivilegeEventService>();
        services.AddTransient<IPrivilegeService, PrivilegeService>();
        services.AddTransient<IAppSecurityMetadataTypeService, AppSecurityMetadataTypeService>();
        services.AddTransient<IRoleEventService, RoleEventService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserEventService, UserEventService>();
        services.AddTransient<IUserRoleEventService, UserRoleEventService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<IUserService, UserService>();
    }

    private static void AddOrchestrations(this IServiceCollection services)
    {
        services.AddTransient<IAppOrchestrationService, AppOrchestrationService>();
        services.AddTransient<IAnalysePlatformUsageOrchestrationService, AnalysePlatformUsageOrchestrationService>();
        services.AddTransient<IPrivilegeOrchestrationService, PrivilegeOrchestrationService>();
        services.AddTransient<IRoleOrchestrationService, RoleOrchestrationService>();
        services.AddTransient<ITokenCleanerOrchestrationService, TokenCleanerOrchestrationService>();
        services.AddTransient<IUserOrchestrationService, UserOrchestrationService>();
        services.AddTransient<IUserRoleOrchestrationService, UserRoleOrchestrationService>();
    }

    private static void AddAggregations(this IServiceCollection services)
    {
        services.AddTransient<IAppSecurityMigrationAggregationService, AppSecurityMigrationAggregationService>();
    }

    private static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddTransient<IAppSecurityEventHandlers, AppSecurityEventHandlers>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IPrivilegeEventProcessingService, PrivilegeEventProcessingService>();
        services.AddTransient<IPrivilegeProcessingService, PrivilegeProcessingService>();
        services.AddTransient<IRoleEventProcessingService, RoleEventProcessingService>();
        services.AddTransient<IRoleProcessingService, RoleProcessingService>();
        services.AddTransient<IUserEventProcessingService, UserEventProcessingService>();
        services.AddTransient<IUserProcessingService, UserProcessingService>();
        services.AddTransient<IUserRoleEventProcessingService, UserRoleEventProcessingService>();
        services.AddTransient<IUserRoleProcessingService, UserRoleProcessingService>();
    }

    private static void ConfigureAppSecurityApiModel(ODataConventionModelBuilder builder) =>
        new AppSecurityModelBuilder(builder).Configure();

    private static void AddApi(
        this IServiceCollection services,
        string routePrefix,
        Action<ODataConventionModelBuilder> configureModel,
        ODataConventionModelBuilder builder = null,
        bool useFullSchemaIds = false)
    {
        services.AddSingleton<Action<ODataConventionModelBuilder>>(configureModel);

        if (builder is not null)
            configureModel(builder);

        AddAspNet(services);

        if (builder is null)
            AddApiDocumentation(services, routePrefix, useFullSchemaIds);

        IEdmModel routeModel = BuildRouteModel(configureModel);
        DefaultODataBatchHandler batchHandler = new();

        services.AddControllers().AddOData(options =>
        {
            options.RouteOptions.EnableQualifiedOperationCall = false;
            options.EnableAttributeRouting = true;
            options.RouteOptions.EnableKeyAsSegment = false;
            options.Expand()
                .Count()
                .Filter()
                .Select()
                .OrderBy()
                .SetMaxTop(1000)
                .AddRouteComponents($"Api/{routePrefix}", routeModel, batchHandler);

            if (builder is null)
                _ = options.AddRouteComponents("Api/Core", routeModel, batchHandler);
        });
    }

    private static void AddApiDocumentation(
        IServiceCollection services,
        string routePrefix,
        bool useFullSchemaIds)
    {
        services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            AddSwaggerDocuments(options, routePrefix);
            options.DocInclusionPredicate(
                (documentName, apiDescription) =>
                    ShouldIncludeInDocument(documentName, apiDescription.RelativePath, routePrefix));

            if (useFullSchemaIds)
                options.CustomSchemaIds(type => type.FullName?.Replace('+', '.') ?? type.Name);

            options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
            {
                Description = @"Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
            });
        });
    }

    private static void AddSwaggerDocuments(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options,
        string routePrefix)
    {
        options.SwaggerDoc(routePrefix, new OpenApiInfo
        {
            Title = $"{routePrefix} API definition",
            Version = routePrefix,
        });
        options.SwaggerDoc("Core", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "Core",
        });
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Core API definition",
            Version = "v1",
        });
    }

    private static bool ShouldIncludeInDocument(
        string documentName,
        string relativePath,
        string routePrefix)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        if (string.Equals(documentName, "v1", StringComparison.OrdinalIgnoreCase))
            documentName = "Core";

        string path = NormalizePath(relativePath);

        return string.Equals(documentName, "Core", StringComparison.OrdinalIgnoreCase)
            ? MatchesContextRoute(path, "Core")
            : MatchesContextRoute(path, routePrefix);
    }

    private static bool MatchesContextRoute(string path, string context)
    {
        string prefix = $"/Api/{context}";
        return path.Equals(prefix, StringComparison.OrdinalIgnoreCase)
            || path.StartsWith($"{prefix}/", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string relativePath) =>
        relativePath.StartsWith('/') ? relativePath : $"/{relativePath}";

    private static IEdmModel BuildRouteModel(Action<ODataConventionModelBuilder> configureModel)
    {
        ODataConventionModelBuilder builder = new();
        configureModel(builder);
        return builder.GetEdmModel();
    }

    private static void AddAspNet(IServiceCollection services)
    {
        services.AddRouting();
        services.AddResponseCompression();
        services.AddHttpClient();
        services.AddHttpContextAccessor();
        services.AddScoped(
            typeof(HttpContext),
            ctx => ctx.GetService<IHttpContextAccessor>()?.HttpContext ?? new DefaultHttpContext());
        services.AddScoped(typeof(HttpRequest), ctx => ctx.GetRequiredService<HttpContext>().Request);
        services.AddSession();
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromMinutes(60);
        });
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddRazorPages();
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
        });
        services.AddEndpointsApiExplorer();
        services.AddSignalR();
    }
}








