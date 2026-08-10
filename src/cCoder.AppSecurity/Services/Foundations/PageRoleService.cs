// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers.Storages;
using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class PageRoleService(IPageRoleBroker pageRoleBroker) : IPageRoleService
{
    public IQueryable<PageRole> GetAll() =>
        TryCatch(operation: IQueryable<PageRole> () =>
        {
            return pageRoleBroker.GetAllPageRoles();
        });

    public int GetPageId(int appId, string path) =>
        TryCatch(operation: int () =>
        {
            ValidatePageIdOnGet(appId: appId, path: path);

            return pageRoleBroker.GetPageId(appId: appId, path: path);
        });

    public ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole) =>
        TryCatch(operation: async ValueTask<PageRole> () =>
        {
            ValidatePageRoleOnAdd(newPageRole: newPageRole);

            return await pageRoleBroker.AddPageRoleAsync(newPageRole: newPageRole);
        });
}