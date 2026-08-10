// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.Data.Models.Packaging;
using cCoder.Security.Models.Events;
using DataPackageItem = cCoder.Data.Models.Packaging.PackageItem;


namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class EventHandlerService(IEventHubBroker eventHubBroker)
    : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: void () =>
        {

            ListenToAppCreateAndUpdateEventsValue();
            ListenToAppDeleteEventsValue();
            ListenToPackageEventsValue();
            ListenToSecurityAccountEventsValue();

        });

    public void ListenToAppCreateAndUpdateEvents() =>
        TryCatch(operation: void () =>
        {

            ListenToAppAddEvents();
            ListenToAppUpdateEvents();

        });

    public void ListenToAppDeleteEvents() =>
        TryCatch(operation: void () =>
        {

            ListenToAppDeleteEvent();
        });

    public void ListenToPackageEvents() =>
        TryCatch(operation: void () =>
        {
            ValidateListenToPackageEvents();

            ListenToPackageEventsValue();
        });

    public void ListenToSecurityAccountEvents() =>
        TryCatch(operation: void () =>
        {

            ListenToSecurityRegistrationCreatedEvent();
            ListenToSecurityRegistrationConfirmedEvent();
            ListenToSecurityInvitationCreatedEvent();
            ListenToSecurityInvitationAcceptedEvent();
            ListenToSecurityPasswordResetRequestedEvent();

        });

    void ListenToAppAddEvents() =>
        eventHubBroker.ListenToEvent<App, IAppRelationshipAggregationService>(
            eventName: "app_add",
            handler: (service, app) => service.AddAppAsync(newApp: app));

    void ListenToAppUpdateEvents() =>
        eventHubBroker.ListenToEvent<App, IAppRelationshipAggregationService>(
            eventName: "app_update",
            handler: (service, app) => service.UpdateAppAsync(updatedApp: app));

    void ListenToAppDeleteEvent() =>
        eventHubBroker.ListenToEvent<App, IAppRelationshipAggregationService>(
            eventName: "app_delete",
            handler: (service, app) => service.DeleteAppAsync(deletedApp: app));

    void ListenToContentPagesImportedEvents() =>
        eventHubBroker.ListenToEvent<AppSecurityPackageEvent, IAppSecurityMigrationAggregationService>(
            eventName: "content_pages_imported",
            handler: (service, packageEvent) => service.ImportPageRolesAppSecurityPackageAsync(
                appId: packageEvent.AppId,
                package: ToLocalPackage(package: packageEvent.Package)));

    void ListenToSecurityRegistrationCreatedEvent() =>
        ListenToSecurityAccountEvent(
            eventName:
                SecurityAccountEventKind.RegistrationCreated
                    .ToEventName());

    void ListenToSecurityRegistrationConfirmedEvent() =>
        ListenToSecurityAccountEvent(
            eventName:
                SecurityAccountEventKind.RegistrationConfirmed
                    .ToEventName());

    void ListenToSecurityInvitationCreatedEvent() =>
        ListenToSecurityAccountEvent(
            eventName:
                SecurityAccountEventKind.InvitationCreated
                    .ToEventName());

    void ListenToSecurityInvitationAcceptedEvent() =>
        ListenToSecurityAccountEvent(
            eventName:
                SecurityAccountEventKind.InvitationAccepted
                    .ToEventName());

    void ListenToSecurityPasswordResetRequestedEvent() =>
        ListenToSecurityAccountEvent(
            eventName:
                SecurityAccountEventKind.PasswordResetRequested
                    .ToEventName());

    void ListenToSecurityAccountEvent(string eventName) =>
        eventHubBroker.ListenToEvent<SecurityAccountEvent, IAccountEventOrchestrationService>(
            eventName: eventName,
            handler: (service, accountEvent) =>
                service.ProcessSecurityAccountEventAsync(accountEvent: accountEvent));

    static AppSecurityPackage ToLocalPackage(Package package) =>
        package == null ? null : new AppSecurityPackage
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Category = package.Category,
            SourceApi = package.SourceApi,
            Items = package.Items?.Select(selector: ToLocalPackageItem)
                .ToArray(),
        };

    static AppSecurityPackageItem ToLocalPackageItem(DataPackageItem packageItem) =>
        packageItem == null ? null : new AppSecurityPackageItem
        {
            Id = packageItem.Id,
            PackageId = packageItem.PackageId,
            Type = packageItem.Type,
            Data = packageItem.Data,
        };

    private void ListenToAppCreateAndUpdateEventsValue() =>
        ListenToAppCreateAndUpdateEvents();

    private void ListenToAppDeleteEventsValue() =>
        ListenToAppDeleteEvents();

    private void ListenToSecurityAccountEventsValue() =>
        ListenToSecurityAccountEvents();

    private void ListenToPackageEventsValue()
    {
        ListenToContentPagesImportedEvents();
    }
}