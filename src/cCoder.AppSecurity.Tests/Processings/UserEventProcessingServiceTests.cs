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

public partial class UserEventProcessingServiceTests
{
    private readonly Mock<IUserEventService> userEventServiceMock;
    private readonly UserEventProcessingService service;

    public UserEventProcessingServiceTests()
    {
        userEventServiceMock = new Mock<IUserEventService>(behavior: MockBehavior.Strict);
        service = new UserEventProcessingService(eventService: userEventServiceMock.Object);
    }

    private static User CreateRandomUser() =>
        Builder<User>.CreateNew().Build();
}