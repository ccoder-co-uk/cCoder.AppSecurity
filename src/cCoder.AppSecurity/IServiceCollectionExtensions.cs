// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Api.OData;
using cCoder.AppSecurity.Exposures;
using cCoder.AppSecurity.Exposures.EventHandlers;
using cCoder.AppSecurity.Exposures.HostedServices;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Packaging;
using cCoder.Data.Models.Security;
using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Security.Objects.Events;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi;
using AuthorizationBroker = cCoder.AppSecurity.Brokers.AuthorizationBroker;
using AuthInfoBroker = cCoder.AppSecurity.Brokers.AuthInfoBroker;
using SecurityDbContextBroker = cCoder.AppSecurity.Brokers.Security.SecurityDbContextBroker;
using TokenBroker = cCoder.AppSecurity.Brokers.Tokens.TokenBroker;
using UIBaselineBroker = cCoder.AppSecurity.Brokers.UIBaselineBroker;
using AppBroker = cCoder.AppSecurity.Brokers.Storages.AppBroker;
using EventHubBroker = cCoder.AppSecurity.Brokers.Events.EventHubBroker;
using IAuthorizationBroker = cCoder.AppSecurity.Brokers.IAuthorizationBroker;
using IAuthInfoBroker = cCoder.AppSecurity.Brokers.IAuthInfoBroker;
using ISecurityDbContextBroker = cCoder.AppSecurity.Brokers.Security.ISecurityDbContextBroker;
using ITokenBroker = cCoder.AppSecurity.Brokers.Tokens.ITokenBroker;
using IUIBaselineBroker = cCoder.AppSecurity.Brokers.IUIBaselineBroker;
using IAppBroker = cCoder.AppSecurity.Brokers.Storages.IAppBroker;
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

public static partial class IServiceCollectionExtensions
{
    public static AppSecurityConfiguration WithEventProviders(
        this AppSecurityConfiguration configuration,
        params EventProvider[] eventProviders)
    {
        configuration.EventProviders = eventProviders ?? [];

        return configuration;
    }

    public static void AddAppSecurityWeb(
        this IServiceCollection services,
        Action<AppSecurityConfiguration> configure = null,
        ODataConventionModelBuilder builder = null) =>
        services.AddConfiguredAppSecurityWeb(configure: (_, configuration) => configure?.Invoke(obj: configuration), builder: builder);

    public static void AddAppSecurityHostedServices(
        this IServiceCollection services,
        Action<AppSecurityConfiguration> configure = null) =>
        services.AddConfiguredAppSecurityHostedServices(configure: (_, configuration) => configure?.Invoke(obj: configuration));

    internal static void AddAppSecurity(this IServiceCollection services)
    {
        services.AddEventingTypes();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddAggregations();
        services.AddEventHandlers();
    }

    private static void AddAppSecurityWeb(this IServiceCollection services, ODataConventionModelBuilder builder = null)
    {
        services.AddAppSecurity();

    }

    private static void AddAppSecurityHostedServices(this IServiceCollection services)
    {
        services.AddAppSecurity();
        services.AddSingleton<IAnalysePlatformUsageHostedService, AnalysePlatformUsageHostedService>();
        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<IAnalysePlatformUsageHostedService>());
        services.AddSingleton<ITokenCleanerHostedService, TokenCleanerHostedService>();
        services.AddSingleton<IHostedService>(implementationFactory: serviceProvider =>
            serviceProvider.GetRequiredService<ITokenCleanerHostedService>());
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
        services.AddEventingForType<SecurityAccountEvent>();
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationBroker, AuthorizationBroker>();
        services.AddTransient<IAuthInfoBroker, AuthInfoBroker>();
        services.AddTransient<ISecurityDbContextBroker, SecurityDbContextBroker>();
        services.AddTransient<ITokenBroker, TokenBroker>();
        services.AddTransient<IUIBaselineBroker, UIBaselineBroker>();
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
        services.AddTransient<IAppBroker, AppBroker>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IUIBaselineService, UIBaselineService>();
        services.AddTransient<IAuthorizationService, AuthorizationService>();
        services.AddTransient<IJsonService, JsonService>();
        services.AddTransient<ITokenCleanerService, TokenCleanerService>();
        services.AddTransient<IAnalysePlatformUsageService, AnalysePlatformUsageService>();
        services.AddTransient<IAppSecurityAppExposure, AppSecurityAppExposure>();
        services.AddTransient<IAppSecurityPackageManager, AppSecurityPackageManager>();
        services.AddTransient<Services.Foundations.Events.IEventHandlerService, Services.Foundations.Events.EventHandlerService>();
        services.AddTransient<IPrivilegeEventService, PrivilegeEventService>();
        services.AddTransient<IPrivilegeService, PrivilegeService>();
        services.AddTransient<IAppSecurityMetadataTypeService, AppSecurityMetadataTypeService>();
        services.AddTransient<IAppService, AppService>();
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
        services.AddTransient<IAccountEventOrchestrationService, AccountEventOrchestrationService>();
        services.AddTransient<IPrivilegeOrchestrationService, PrivilegeOrchestrationService>();
        services.AddTransient<IRoleOrchestrationService, RoleOrchestrationService>();
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
        services.AddTransient<IAppProcessingService, AppProcessingService>();
        services.AddTransient<IAnalysePlatformUsageProcessingService, AnalysePlatformUsageProcessingService>();
        services.AddTransient<IJsonProcessingService, JsonProcessingService>();
        services.AddTransient<IPrivilegeEventProcessingService, PrivilegeEventProcessingService>();
        services.AddTransient<IPrivilegeProcessingService, PrivilegeProcessingService>();
        services.AddTransient<IRoleEventProcessingService, RoleEventProcessingService>();
        services.AddTransient<IRoleProcessingService, RoleProcessingService>();
        services.AddTransient<IUserEventProcessingService, UserEventProcessingService>();
        services.AddTransient<IUserProcessingService, UserProcessingService>();
        services.AddTransient<IUserRoleEventProcessingService, UserRoleEventProcessingService>();
        services.AddTransient<IUserRoleProcessingService, UserRoleProcessingService>();
    }
}
