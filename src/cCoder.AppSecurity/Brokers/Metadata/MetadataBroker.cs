// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.AppSecurity.Api.OData;

namespace cCoder.AppSecurity.Brokers.Metadata;

internal static class MetadataBroker
{
    private static readonly Dictionary<Type, string> Lookup = new()
    {
        { typeof(short), "number" },
        { typeof(int), "number" },
        { typeof(long), "number" },
        { typeof(short?), "number" },
        { typeof(int?), "number" },
        { typeof(long?), "number" },
        { typeof(ushort), "number" },
        { typeof(uint), "number" },
        { typeof(ulong), "number" },
        { typeof(ushort?), "number" },
        { typeof(uint?), "number" },
        { typeof(ulong?), "number" },
        { typeof(byte), "number" },
        { typeof(byte?), "number" },
        { typeof(decimal), "number" },
        { typeof(decimal?), "number" },
        { typeof(string), "string" },
        { typeof(DateTime), "date" },
        { typeof(DateTime?), "date" },
        { typeof(TimeSpan), "time" },
        { typeof(TimeSpan?), "time" },
        { typeof(DateTimeOffset), "date" },
        { typeof(DateTimeOffset?), "date" },
        { typeof(Guid), "guid" },
        { typeof(Guid?), "guid" },
        { typeof(bool), "bool" },
        { typeof(bool?), "bool" },
        { typeof(double), "number" },
        { typeof(double?), "number" },
        { typeof(float), "number" },
        { typeof(float?), "number" },
    };

    public static MetadataContainer CreateMetadataContainer(
        Type type,
        bool isEntity,
        bool hasEndpoint)
    {
        bool isValueType = type.IsValueType | type == typeof(string);

        Func<PropertyContainer[]>[] propertySelectors =
        [
            () => type.GetProperties()
                .Select(selector: CreatePropertyContainer)
                .ToArray(),
            () => [],
        ];

        PropertyContainer[] properties =
            propertySelectors[Convert.ToInt32(value: isValueType)]
                .Invoke();

        return new MetadataContainer
        {
            IsValueType = isValueType,
            Type = GetTypeName(type: type),
            Name = type.Name,
            DisplayName = type.Name,
            Description = type.Name,
            ServerType = type.AssemblyQualifiedName,
            ServerTypeName = type.GetCSharpTypeName(),
            Properties = properties,
            IsEntity = isEntity,
            IsJoinEntity = isEntity & type.IsJoinType(),
            HasEndpoint = hasEndpoint,
        };
    }

    public static ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        bool isEntity,
        bool hasEndpoint)
    {
        MetadataContainer metadata = CreateMetadataContainer(
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

        return new ExtendedMetadataContainer
        {
            Type = metadata.Type,
            ServerTypeName = metadata.ServerTypeName,
            IsValueType = metadata.IsValueType,
            IsEntity = metadata.IsEntity,
            IsJoinEntity = metadata.IsJoinEntity,
            HasEndpoint = metadata.HasEndpoint,
            IsSystemManaged = metadata.IsSystemManaged,
            Category = metadata.Category,
            Name = metadata.Name,
            DisplayName = metadata.DisplayName,
            Description = metadata.Description,
            ServerType = metadata.ServerType,
            Properties = metadata.Properties,
        };
    }

    private static PropertyContainer CreatePropertyContainer(PropertyInfo property)
    {
        bool isNullableType = property.PropertyType.IsGenericType
            && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);

        return new PropertyContainer
        {
            Name = property.Name,
            Type = GetTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = property.PropertyType.GetCSharpTypeName(),
            IsValueType = property.PropertyType.IsValueType | property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = property.GetCustomAttribute<KeyAttribute>() != null | property.Name == "Id"
                ? "key"
                : property.Name,
            IsRequired = (!isNullableType & property.PropertyType.IsValueType)
                | property.GetCustomAttribute<RequiredAttribute>() != null,
        };
    }

    private static string GetTypeName(Type type)
    {
        Func<string>[] typeNameSelectors =
        [
            () => Lookup.GetValueOrDefault(key: type, defaultValue: "object"),
            () => "array",
            () => "string",
        ];

        bool isString = type == typeof(string);
        bool isEnumerable = typeof(IEnumerable).IsAssignableFrom(c: type);
        int selectorIndex = Convert.ToInt32(value: isString) * 2;
        selectorIndex += Convert.ToInt32(value: isEnumerable & !isString);

        return typeNameSelectors[selectorIndex].Invoke();
    }
}