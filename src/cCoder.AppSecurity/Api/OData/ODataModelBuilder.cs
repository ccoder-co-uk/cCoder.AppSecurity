// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq.Expressions;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using Microsoft.OData.ModelBuilder;

namespace cCoder.AppSecurity.Api.OData;

public abstract class ODataModelBuilder
{
    protected ODataConventionModelBuilder Builder { get; }

    protected ODataModelBuilder(ODataConventionModelBuilder builder = null)
    {
        Builder = builder ?? new ODataConventionModelBuilder();
    }

    public abstract ODataModel Build();

    protected virtual EntitySetConfiguration<T> AddSet<T, TKey>(
        bool enableBatchingToo = false,
        string setName = null)
        where T : class
    {
        setName ??= typeof(T).Name;
        return Builder.EntitySet<T>(setName);
    }

    protected virtual EntitySetConfiguration<T> AddJoinSet<T, TKey>(Expression<Func<T, TKey>> key)
        where T : class
    {
        string setName = typeof(T).Name;
        EntitySetConfiguration<T> setConfig = Builder.EntitySet<T>(setName);
        _ = Builder.EntityType<T>().HasKey(key);
        return setConfig;
    }

    protected virtual void AddCommonComplextypes()
    {
        _ = Builder.ComplexType<MetadataContainerSet>();
        _ = Builder.ComplexType<MetadataContainer>();
        _ = Builder.ComplexType<PropertyContainer>();
        _ = Builder.ComplexType<AuditResultsByUser>();
        _ = Builder.ComplexType<AuditResultByProperty>();
    }
}