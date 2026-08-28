// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.AppSecurity.Tests;

public sealed partial class ConfigurationOwnershipTests
{
    [Fact]
    public void AppSecurityConfiguration_ShouldNotOwnPersistenceConfiguration()
    {
        // Given
        Type configurationType = typeof(AppSecurityConfiguration);

        // When
        string[] propertyNames = configurationType
            .GetProperties()
            .Select(selector: property => property.Name)
            .ToArray();

        // Then
        propertyNames.Should()
            .NotContain(unexpected: [
                "ConnectionString",
                "DebugInfo",
                "LogSQL"]);
    }

    [Fact]
    public void AddAppSecurityWeb_ShouldNotRegisterCoreDataServices()
    {
        // Given
        IServiceCollection services = new ServiceCollection();
        AppSecurityConfiguration configuration = new();

        typeof(AppSecurityConfiguration)
            .GetProperty(name: "ConnectionString")
            ?.SetValue(obj: configuration, value: "Server=(local);");

        // When
        services.AddAppSecurityWeb(configuration: configuration);

        // Then
        services.Should()
            .NotContain(predicate: descriptor =>
                descriptor.ServiceType == typeof(CoreDataContext));
    }
}