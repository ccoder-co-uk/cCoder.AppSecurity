// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Events;
using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.Security.Models.Events;

namespace cCoder.AppSecurity.Services.Foundations.Events;

internal sealed partial class EventHandlerService(IEventHubBroker eventHubBroker)
    : IEventHandlerService
{
    public void ListenToAllEvents() =>
        TryCatch(operation: void () =>
        {

            ListenToAppCreateAndUpdateEventsValue();
            ListenToAppDeleteEventsValue();
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

    private void ListenToAppCreateAndUpdateEventsValue() =>
        ListenToAppCreateAndUpdateEvents();

    private void ListenToAppDeleteEventsValue() =>
        ListenToAppDeleteEvents();

    private void ListenToSecurityAccountEventsValue() =>
        ListenToSecurityAccountEvents();

}