using EventLibrary;


namespace cCoder.AppSecurity.Brokers.Events;

internal class EventHubBroker(IEventHub eventHub) : IEventHubBroker
{
    public void ListenToEvent<T, TService>(string eventName, Func<TService, T, ValueTask> handler) =>
        eventHub.ListenToEvent(eventName, handler);
}

