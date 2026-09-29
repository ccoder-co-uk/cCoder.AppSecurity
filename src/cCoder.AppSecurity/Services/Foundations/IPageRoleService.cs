// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Services.Foundations;

internal interface IPageRoleService
{
    IQueryable<PageRole> GetAll();
    int GetPageId(int appId, string path);
    ValueTask<PageRole> AddPageRoleAsync(PageRole newPageRole);
}