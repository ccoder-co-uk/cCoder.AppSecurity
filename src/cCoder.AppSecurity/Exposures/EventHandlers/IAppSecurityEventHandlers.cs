// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Exposures.EventHandlers;

public interface IAppSecurityEventHandlers
{
    void ListenToAllEvents();
    void ListenToAppCreateAndUpdateEvents();
    void ListenToAppDeleteEvents();
    void ListenToSecurityAccountEvents();
}