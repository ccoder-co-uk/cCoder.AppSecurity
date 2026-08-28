// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models;
using cCoder.Security.Models;
using Xunit;

namespace Web.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void ShouldExposeEveryRequiredDomainConfiguration()
    {
        // Given
        const string typeName =
            "AppSecurity.Web.Models.AppConfiguration, AppSecurity.Web";

        // When
        Type configurationType = Type.GetType(typeName: typeName);

        // Then
        Assert.NotNull(@object: configurationType);

        Assert.Equal(
            expected: typeof(AppSecurityConfiguration),
            actual: configurationType.GetProperty(name: "AppSecurity")?.PropertyType);

        Assert.Equal(
            expected: typeof(CoreDataConfiguration),
            actual: configurationType.GetProperty(name: "CoreData")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityConfiguration),
            actual: configurationType.GetProperty(name: "Security")?.PropertyType);

        Assert.Equal(
            expected: typeof(SecurityDataConfiguration),
            actual: configurationType.GetProperty(name: "SecurityData")?.PropertyType);
    }
}