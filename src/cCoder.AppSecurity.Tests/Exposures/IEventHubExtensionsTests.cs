// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Reflection;
using cCoder.AppSecurity.Services.Aggregations;
using cCoder.AppSecurity.Services.Orchestrations;
using cCoder.Data.Models.CMS;
using cCoder.Eventing;
using cCoder.Security.Models.Events;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Exposures;

public sealed partial class IEventHubExtensionsTests
{
    [Fact]
    public void ListenToAppSecurityEvents_WhenInvoked_RegistersExpectedListenerMap()
    {
        // Given
        Mock<IEventHub> eventHubMock = new(behavior: MockBehavior.Loose);

        Type extensionsType = typeof(WebApplicationExtensions).Assembly.GetType(
            name: "cCoder.AppSecurity.IEventHubExtensions");

        MethodInfo listenMethod = extensionsType?.GetMethod(
            name: "ListenToAppSecurityEvents",
            bindingAttr: BindingFlags.Public | BindingFlags.Static);

        // When
        listenMethod?.Invoke(
            obj: null,
            parameters: [eventHubMock.Object]);

        // Then
        extensionsType
            .Should()
            .NotBeNull();

        listenMethod
            .Should()
            .NotBeNull();

        ListenerRegistration[] actualRegistrations = eventHubMock.Invocations
            .Select(selector: invocation => new ListenerRegistration(
                EventName: invocation.Arguments[0] as string,
                PayloadType: invocation.Method.GetGenericArguments()[0],
                ServiceType: invocation.Method.GetGenericArguments()[1]))
            .ToArray();

        ListenerRegistration[] expectedRegistrations =
        [
            new("app_add", typeof(App), typeof(IAppRelationshipAggregationService)),
            new("app_update", typeof(App), typeof(IAppRelationshipAggregationService)),
            new("app_delete", typeof(App), typeof(IAppRelationshipAggregationService)),
            new("security_account_registration_created", typeof(SecurityAccountEvent), typeof(IAccountEventOrchestrationService)),
            new("security_account_registration_confirmed", typeof(SecurityAccountEvent), typeof(IAccountEventOrchestrationService)),
            new("security_account_invitation_created", typeof(SecurityAccountEvent), typeof(IAccountEventOrchestrationService)),
            new("security_account_invitation_accepted", typeof(SecurityAccountEvent), typeof(IAccountEventOrchestrationService)),
            new("security_account_password_reset_requested", typeof(SecurityAccountEvent), typeof(IAccountEventOrchestrationService)),
        ];

        actualRegistrations
            .Should()
            .BeEquivalentTo(
                expectation: expectedRegistrations,
                config: options => options.WithStrictOrdering());
    }

    [Fact]
    public void AppSecurityEventListening_WhenInspected_HasNoIntermediateRegistrationTypes()
    {
        // Given
        Assembly assembly = typeof(WebApplicationExtensions).Assembly;

        string[] retiredTypeNames =
        [
            "cCoder.AppSecurity.Exposures.EventHandlers.IAppSecurityEventHandlers",
            "cCoder.AppSecurity.Exposures.EventHandlers.AppSecurityEventHandlers",
            "cCoder.AppSecurity.Services.Foundations.Events.IEventHandlerService",
            "cCoder.AppSecurity.Services.Foundations.Events.EventHandlerService",
            "cCoder.AppSecurity.Brokers.Events.IEventHubBroker",
            "cCoder.AppSecurity.Brokers.Events.EventHubBroker",
        ];

        // When
        Type[] registrationTypes = retiredTypeNames
            .Select(selector: assembly.GetType)
            .Where(predicate: type => type is not null)
            .ToArray();

        // Then
        registrationTypes
            .Should()
            .BeEmpty();
    }

    private sealed record ListenerRegistration(
        string EventName,
        Type PayloadType,
        Type ServiceType);
}