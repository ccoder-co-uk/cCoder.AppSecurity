// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IPageRoleService
{
    IQueryable<PageRole> GetAll();
    int GetPageId(int appId, string path);
    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);
}