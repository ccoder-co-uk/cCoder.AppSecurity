// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models.Exceptions;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.AppSecurity.Services.Processings;
using cCoder.Data.Models.CMS;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Processings;

public sealed partial class AppProcessingServiceTests
{
    private readonly Mock<IAppService> appServiceMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        RoleProcessingServiceExceptionTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnAppsWhenGetAll()
    {
        // Given
        App app = new() { Id = 7 };

        appServiceMock
            .Setup(expression: service => service.GetAll())
            .Returns(value: new[] { app }.AsQueryable());

        AppProcessingService service = CreateService();

        // When
        IQueryable<App> result = service.GetAll();

        // Then
        result
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: app);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetAllFailure(Exception exception, Type expectedType)
    {
        // Given
        appServiceMock
            .Setup(expression: service => service.GetAll())
            .Throws(exception: exception);

        AppProcessingService service = CreateService();

        // When
        Action action = () => service.GetAll();

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public void ShouldReturnAppWhenGetByDomain()
    {
        // Given
        App app = new() { Id = 7, Domain = "example.test" };

        appServiceMock
            .Setup(expression: service => service.GetByDomain(
                domain: "example.test"))
            .Returns(value: app);

        AppProcessingService service = CreateService();

        // When
        App result = service.GetByDomain(domain: "example.test");

        // Then
        result
            .Should()
            .BeSameAs(expected: app);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetByDomainFailure(Exception exception, Type expectedType)
    {
        // Given
        appServiceMock
            .Setup(expression: service => service.GetByDomain(
                domain: "example.test"))
            .Throws(exception: exception);

        AppProcessingService service = CreateService();

        // When
        Action action = () => service.GetByDomain(domain: "example.test");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private AppProcessingService CreateService() =>
        new(service: appServiceMock.Object);
}