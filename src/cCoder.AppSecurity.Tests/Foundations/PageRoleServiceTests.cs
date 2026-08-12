// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data.Models.Security;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.AppSecurity.Tests.Foundations;

public sealed partial class PageRoleServiceTests
{
    private readonly Mock<IPageRoleBroker> pageRoleBrokerMock = new();

    public static TheoryData<Exception, Type> ExceptionMappings =>
        UserRoleServiceExceptionTests.ExceptionMappings;

    [Fact]
    public void ShouldReturnPageRolesWhenGetAll()
    {
        // Given
        PageRole pageRole = new();

        pageRoleBrokerMock
            .Setup(expression: broker => broker.GetAllPageRoles())
            .Returns(value: new[] { pageRole }.AsQueryable());

        PageRoleService service = CreateService();

        // When
        IQueryable<PageRole> result = service.GetAll();

        // Then
        result
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: pageRole);
    }

    [Fact]
    public void ShouldReturnPageIdWhenGetPageId()
    {
        // Given
        pageRoleBrokerMock
            .Setup(expression: broker => broker.GetPageId(
                appId: 7,
                path: "/home"))
            .Returns(value: 11);

        PageRoleService service = CreateService();

        // When
        int result = service.GetPageId(appId: 7, path: "/home");

        // Then
        result
            .Should()
            .Be(expected: 11);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public void ShouldMapGetPageIdFailure(Exception exception, Type expectedType)
    {
        // Given
        pageRoleBrokerMock
            .Setup(expression: broker => broker.GetPageId(
                appId: 7,
                path: "/home"))
            .Throws(exception: exception);

        PageRoleService service = CreateService();

        // When
        Action action = () => service.GetPageId(appId: 7, path: "/home");

        // Then
        action
            .Should()
            .Throw<Exception>()
            .Which
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    [Fact]
    public async Task ShouldReturnPageRoleWhenAddPageRoleAsync()
    {
        // Given
        PageRole pageRole = new();

        pageRoleBrokerMock
            .Setup(expression: broker => broker.AddPageRoleAsync(
                newPageRole: pageRole))
            .ReturnsAsync(value: pageRole);

        PageRoleService service = CreateService();

        // When
        PageRole result = await service.AddPageRoleAsync(newPageRole: pageRole);

        // Then
        result
            .Should()
            .BeSameAs(expected: pageRole);
    }

    [Theory]
    [MemberData(nameof(ExceptionMappings))]
    public async Task ShouldMapAddPageRoleAsyncFailure(Exception exception, Type expectedType)
    {
        // Given
        PageRole pageRole = new();

        pageRoleBrokerMock
            .Setup(expression: broker => broker.AddPageRoleAsync(
                newPageRole: pageRole))
            .Throws(exception: exception);

        PageRoleService service = CreateService();

        // When
        Func<Task> action = async () => await service.AddPageRoleAsync(
            newPageRole: pageRole);

        // Then
        Exception thrown = (await action
            .Should()
            .ThrowAsync<Exception>()).Which;

        thrown
            .Should()
            .BeOfType(expectedType: expectedType);
    }

    private PageRoleService CreateService() =>
        new(pageRoleBroker: pageRoleBrokerMock.Object);
}