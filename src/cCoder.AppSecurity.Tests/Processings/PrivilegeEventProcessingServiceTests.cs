// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations.Events;
using cCoder.AppSecurity.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Security.Processings;

public partial class PrivilegeEventProcessingServiceTests
{
    private readonly Mock<IPrivilegeEventService> privilegeEventServiceMock;
    private readonly PrivilegeEventProcessingService service;

    public PrivilegeEventProcessingServiceTests()
    {
        privilegeEventServiceMock = new Mock<IPrivilegeEventService>(behavior: MockBehavior.Strict);
        service = new PrivilegeEventProcessingService(eventService: privilegeEventServiceMock.Object);
    }

    private static Privilege CreateRandomPrivilege() =>
        Builder<Privilege>.CreateNew().Build();
}