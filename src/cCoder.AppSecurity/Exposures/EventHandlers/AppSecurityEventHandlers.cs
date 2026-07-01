using cCoder.AppSecurity.Services.Foundations.Events;


namespace cCoder.AppSecurity.Exposures.EventHandlers;

internal class AppSecurityEventHandlers(IEventHandlerService eventHandlerService) : IAppSecurityEventHandlers
{
    public void ListenToAllEvents() => eventHandlerService.ListenToAllEvents();

    public void ListenToAppCreateAndUpdateEvents() => eventHandlerService.ListenToAppCreateAndUpdateEvents();

    public void ListenToAppDeleteEvents() => eventHandlerService.ListenToAppDeleteEvents();

    public void ListenToSecurityAccountEvents() => eventHandlerService.ListenToSecurityAccountEvents();
}

