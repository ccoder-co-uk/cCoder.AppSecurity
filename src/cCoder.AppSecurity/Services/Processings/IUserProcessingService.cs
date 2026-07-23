// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;


namespace cCoder.AppSecurity.Services.Processings;

public interface IUserProcessingService
{
    User Get(string id);
    User GetByEmail(string email, bool ignoreFilters = false);
    IQueryable<User> GetAll(bool ignoreFilters = false);
    ValueTask<User> AddUserAsync(User entity);
    ValueTask<User> UpdateUserAsync(User entity);
    ValueTask DeleteAsync(string id);
    ValueTask<IEnumerable<Result<User>>> AddOrUpdateUser(
        IEnumerable<User> items
    );
    ValueTask DeleteAllUserAsync(IEnumerable<User> items);
}