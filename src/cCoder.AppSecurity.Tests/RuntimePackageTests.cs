// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace cCoder.AppSecurity.Tests;

public sealed partial class RuntimePackageTests
{
    [Fact]
    public void RuntimeOutput_WhenAnalyzerIsNotDeployed_DoesNotContainAnalyzerAssembly()
    {
        // Given

        string runtimeOutputDirectory = Path.GetDirectoryName(
            path: typeof(IServiceCollectionExtensions).Assembly.Location);

        string analyzerAssemblyPath = Path.Combine(
            path1: runtimeOutputDirectory,
            path2: "cCoder.CodeAnalysis.dll");

        // When

        bool analyzerAssemblyExists = File.Exists(path: analyzerAssemblyPath);

        // Then

        analyzerAssemblyExists
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData("cCoder.Data", "2026.9.23.1706")]
    [InlineData("cCoder.Eventing", "2026.9.23.1651")]
    [InlineData("cCoder.Security", "2026.9.23.1720")]
    [InlineData("cCoder.Security.Data", "2026.9.23.1720")]
    public void RuntimeDependencies_WhenResolved_UseCurrentFoundationVersion(
        string packageName,
        string expectedVersion)
    {
        // Given

        string dependencyManifestPath = Path.ChangeExtension(
            path: typeof(RuntimePackageTests).Assembly.Location,
            extension: ".deps.json");

        using JsonDocument dependencyManifest = JsonDocument.Parse(
            json: File.ReadAllText(path: dependencyManifestPath));

        // When

        string resolvedVersion = dependencyManifest.RootElement
            .GetProperty(propertyName: "libraries")
            .EnumerateObject()
            .Select(selector: dependency => dependency.Name.Split(separator: '/'))
            .Where(predicate: identity => identity[0] == packageName)
            .Select(selector: identity => identity[1])
            .Single();

        // Then

        resolvedVersion
            .Should()
            .Be(expected: expectedVersion);
    }
}