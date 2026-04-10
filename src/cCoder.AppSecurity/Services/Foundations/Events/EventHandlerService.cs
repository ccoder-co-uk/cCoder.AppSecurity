using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.Data.Models.Packaging;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal class EventHandlerService(IEventHubBroker eventHubBroker)
    : IEventHandlerService
{
    public void ListenToAllEvents()
    {
        ListenToAppCreateAndUpdateEvents();
        ListenToAppDeleteEvents();
        ListenToPackageEvents();
    }

    public void ListenToAppCreateAndUpdateEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
    }

    public void ListenToAppDeleteEvents() =>
        ListenToAppDeleteEvent();

    void ListenToPackageEvents() => ListenToPackageImportEvents();

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
            "app_add",
            (service, app) => service.AddAsync(app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
            "app_update",
            (service, app) => service.UpdateAsync(app));

    void ListenToAppDeleteEvent() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
            "app_delete",
            (service, app) => service.DeleteAsync(app.Id));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IAppSecurityMigrationAggregationService>(
            "package_import",
            (service, args) => service.ImportPackageAsync(args.appId, ToLocalPackage(args.package)));

    static AppSecurityPackage ToLocalPackage(Package package) =>
        package == null ? null : new AppSecurityPackage(package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(ToLocalPackageItem).ToArray(),
        };

    static AppSecurityPackageItem ToLocalPackageItem(DataPackageItem packageItem) =>
        packageItem == null ? null : new AppSecurityPackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data,
        };
}


