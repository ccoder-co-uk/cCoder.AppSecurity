// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Events;

namespace cCoder.AppSecurity.Services.Processings.Events;

internal interface IAccountEventProcessingService
{
    ValueTask ProcessSecurityAccountEventAsync(SecurityAccountEvent accountEvent);
}