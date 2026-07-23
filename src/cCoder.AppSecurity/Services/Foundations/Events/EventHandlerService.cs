// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Processings.Events;
using cCoder.Data.Models.Packaging;
using cCoder.Security.Objects.Events;
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
        ListenToSecurityAccountEvents();
    }

    public void ListenToAppCreateAndUpdateEvents()
    {
        ListenToAppAddEvents();
        ListenToAppUpdateEvents();
    }

    public void ListenToAppDeleteEvents() =>
        ListenToAppDeleteEvent();

    void ListenToPackageEvents() =>
        ListenToPackageImportEvents();

    public void ListenToSecurityAccountEvents()
    {
        ListenToSecurityRegistrationCreatedEvent();
        ListenToSecurityRegistrationConfirmedEvent();
        ListenToSecurityInvitationCreatedEvent();
        ListenToSecurityInvitationAcceptedEvent();
        ListenToSecurityPasswordResetRequestedEvent();
    }

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
eventName: "app_add",
handler: (service, app) => service.AddAsync(app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
eventName: "app_update",
handler: (service, app) => service.UpdateAsync(app));

    void ListenToAppDeleteEvent() =>
        eventHubBroker.ListenToEvent<App, Services.Orchestrations.IAppOrchestrationService>(
eventName: "app_delete",
handler: (service, app) => service.DeleteAsync(app.Id));

    void ListenToPackageImportEvents() =>
        eventHubBroker.ListenToEvent<(int appId, Package package), IAppSecurityMigrationAggregationService>(
eventName: "package_import",
handler: (service, args) => service.ImportPackageAsync(args.appId, ToLocalPackage(args.package)));

    void ListenToSecurityRegistrationCreatedEvent() =>
        ListenToSecurityAccountEvent(eventName: SecurityAccountEventNames.RegistrationCreated);

    void ListenToSecurityRegistrationConfirmedEvent() =>
        ListenToSecurityAccountEvent(eventName: SecurityAccountEventNames.RegistrationConfirmed);

    void ListenToSecurityInvitationCreatedEvent() =>
        ListenToSecurityAccountEvent(eventName: SecurityAccountEventNames.InvitationCreated);

    void ListenToSecurityInvitationAcceptedEvent() =>
        ListenToSecurityAccountEvent(eventName: SecurityAccountEventNames.InvitationAccepted);

    void ListenToSecurityPasswordResetRequestedEvent() =>
        ListenToSecurityAccountEvent(eventName: SecurityAccountEventNames.PasswordResetRequested);

    void ListenToSecurityAccountEvent(string eventName) =>
        eventHubBroker.ListenToEvent<SecurityAccountEvent, IAccountEventProcessingService>(
eventName: eventName,
handler: (service, accountEvent) => service.ProcessAsync(accountEvent));

    static AppSecurityPackage ToLocalPackage(Package package) =>
        package == null ? null : new AppSecurityPackage(name: package.Name)
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(selector: ToLocalPackageItem).ToArray(),
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