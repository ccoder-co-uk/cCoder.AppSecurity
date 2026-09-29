// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Data.Models.Security;

namespace cCoder.AppSecurity.Exposures;

public interface IUserManager
{
    User Get(string id);
    User GetByEmail(string email, bool ignoreFilters = false);
    IQueryable<User> GetAll(bool ignoreFilters = false);
    ValueTask<User> AddUserAsync(User entity);
    ValueTask<User> UpdateUserAsync(User entity);
    ValueTask DeleteAsync(string id);
    ValueTask DeleteAllUserAsync(IEnumerable<User> items);
}