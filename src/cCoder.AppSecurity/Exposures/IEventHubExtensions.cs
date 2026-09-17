// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Security.Models.Events;

namespace cCoder.AppSecurity;

public static class IEventHubExtensions
{
    public static void ListenToAppSecurityEvents(this IEventHub eventHub)
    {
        ListenToAppEvents(eventHub: eventHub);
        ListenToSecurityAccountEvents(eventHub: eventHub);
    }

    private static void ListenToAppEvents(IEventHub eventHub)
    {
        eventHub.ListenToEvent<App, IAppRelationshipAggregationService>(
            name: "app_add",
            handler: static (service, app) =>
                service.AddAppAsync(newApp: app));

        eventHub.ListenToEvent<App, IAppRelationshipAggregationService>(
            name: "app_update",
            handler: static (service, app) =>
                service.UpdateAppAsync(updatedApp: app));

        eventHub.ListenToEvent<App, IAppRelationshipAggregationService>(
            name: "app_delete",
            handler: static (service, app) =>
                service.DeleteAppAsync(deletedApp: app));
    }

    private static void ListenToSecurityAccountEvents(IEventHub eventHub)
    {
        ListenToSecurityAccountEvent(
            eventHub: eventHub,
            eventName: SecurityAccountEventKind.RegistrationCreated.ToEventName());

        ListenToSecurityAccountEvent(
            eventHub: eventHub,
            eventName: SecurityAccountEventKind.RegistrationConfirmed.ToEventName());

        ListenToSecurityAccountEvent(
            eventHub: eventHub,
            eventName: SecurityAccountEventKind.InvitationCreated.ToEventName());

        ListenToSecurityAccountEvent(
            eventHub: eventHub,
            eventName: SecurityAccountEventKind.InvitationAccepted.ToEventName());

        ListenToSecurityAccountEvent(
            eventHub: eventHub,
            eventName: SecurityAccountEventKind.PasswordResetRequested.ToEventName());
    }

    private static void ListenToSecurityAccountEvent(
        IEventHub eventHub,
        string eventName) =>
        eventHub.ListenToEvent<
            SecurityAccountEvent,
            IAccountEventOrchestrationService>(
                name: eventName,
                handler: static (service, accountEvent) =>
                    service.ProcessSecurityAccountEventAsync(
                        securityAccountEvent: accountEvent));
}