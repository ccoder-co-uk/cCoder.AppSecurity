// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Api.OData;

public class MetadataContainerSet
{
    public string Name { get; set; }
    public string UriBase { get; set; }
    public ExtendedMetadataContainer[] Types { get; set; }
}

public class MetadataContainer
{
    public string Type { get; set; }
    public string ServerTypeName { get; set; }
    public bool IsValueType { get; set; }
    public bool IsEntity { get; set; }
    public bool IsJoinEntity { get; set; }
    public bool HasEndpoint { get; set; }
    public bool IsSystemManaged { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string ServerType { get; set; }
    public IEnumerable<PropertyContainer> Properties { get; set; }
}

public class ExtendedMetadataContainer : MetadataContainer
{
    public IEnumerable<OperationContainer> Operations { get; set; }
}

public class PropertyContainer
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ServerType { get; set; }
    public string ServerTypeName { get; set; }
    public string Template { get; set; }
    public string DisplayName { get; set; }
    public string ShortDisplayName { get; set; }
    public string Description { get; set; }
    public bool IsGeneric { get; set; }
    public bool IsValueType { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsRequired { get; set; }
    public bool IsSystemManaged { get; set; }
}

public class OperationContainer
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Definition { get; set; }
    public string HttpVerb { get; set; }
    public bool Queryable { get; set; }
    public MetadataContainer ReturnType { get; set; }
    public IDictionary<string, string> Parameters { get; set; }
}