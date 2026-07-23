// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Services.Foundations.Events;

public interface IEventHandlerService
{
    void ListenToAllEvents();
    void ListenToAppCreateAndUpdateEvents();
    void ListenToAppDeleteEvents();
    void ListenToSecurityAccountEvents();
}