// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Foundations;

public interface IUserService
{
    User Get(string id);
    User GetByEmail(string email, bool ignoreFilters = false);
    IQueryable<User> GetAll(bool ignoreFilters = false);
    ValueTask<User> AddAsync(User user);
    ValueTask<User> UpdateAsync(User user);
    ValueTask DeleteAsync(string id);
}