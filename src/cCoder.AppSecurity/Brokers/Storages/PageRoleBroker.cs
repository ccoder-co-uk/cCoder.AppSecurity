// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data;
using cCoder.Data.Models.Security;
using Microsoft.EntityFrameworkCore;

namespace cCoder.AppSecurity.Brokers.Storages;

internal interface IPageRoleBroker
{
    IQueryable<PageRole> GetAllPageRoles();
    int GetPageId(int appId, string path);
    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);
}

internal sealed class PageRoleBroker(ICoreContextFactory coreContextFactory) : IPageRoleBroker
{
    public IQueryable<PageRole> GetAllPageRoles()
    {
        CoreDataContext context = coreContextFactory.CreateCoreContext();
        return context.PageRoles.IgnoreQueryFilters();
    }

    public int GetPageId(int appId, string path)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();

        return context.Pages
            .IgnoreQueryFilters()
            .Where(predicate: page => page.AppId == appId && page.Path == path)
            .Select(selector: page => page.Id)
            .FirstOrDefault();
    }

    public async ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole)
    {
        using CoreDataContext context = coreContextFactory.CreateCoreContext();
        PageRole result = (await context.PageRoles.AddAsync(entity: newPageRole)).Entity;
        _ = await context.SaveChangesAsync();
        return result;
    }
}