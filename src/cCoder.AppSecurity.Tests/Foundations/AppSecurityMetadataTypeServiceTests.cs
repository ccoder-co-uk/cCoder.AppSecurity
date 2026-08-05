// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Services.Foundations;
using FluentAssertions;
using Xunit;


namespace cCoder.Core.Services.Tests.Security.Foundations;

public sealed partial class AppSecurityMetadataTypeServiceTests
{
    [Fact]
    public void ShouldReturnAppSecurityEntityMetadata()
    {
        // Given
        var service = new AppSecurityMetadataTypeService();

        // When
        var metadataSets = service.GetKnownMetadata()
            .ToArray();

        // Then
        metadataSets.Should()
            .ContainSingle();

        var metadataSet = metadataSets.Single();

        metadataSet.Name.Should()
            .Be(expected: "AppSecurity");

        metadataSet.UriBase.Should()
            .Be(expected: "AppSecurity");

        metadataSet.Types.Should()
            .HaveCount(expected: 4);

        metadataSet.Types.Should()
            .OnlyContain(predicate: metadata =>
                metadata.Category == "AppSecurity" &&
                metadata.IsEntity &&
                metadata.HasEndpoint);

        metadataSet.Types.Select(selector: metadata => metadata.Name)
            .Should()
            .BeEquivalentTo(expectation: ["Privilege", "Role", "User", "UserRole"]);
    }
}