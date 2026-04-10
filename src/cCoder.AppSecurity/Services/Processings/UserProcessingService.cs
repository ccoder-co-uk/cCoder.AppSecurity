using System.Security;
using cCoder.AppSecurity.Models;
using cCoder.Data.Models.CMS;
using cCoder.Data.Models.Security;
using cCoder.AppSecurity.Services.Foundations;
using cCoder.Data;


namespace cCoder.AppSecurity.Services.Processings;

internal class UserProcessingService(IUserService service, ICoreAuthInfo authInfo)
    : IUserProcessingService
{
    public User Get(string id) => service.Get(id);

    public User GetByEmail(string email, bool ignoreFilters = false) =>
        service.GetByEmail(email, ignoreFilters);

    public IQueryable<User> GetAll(bool ignoreFilters = false) => service.GetAll(ignoreFilters);

    public async ValueTask<User> AddAsync(User newUser)
    {
        User existingUser = service
            .GetAll(true)
            .FirstOrDefault(u => u.Id == newUser.Id || u.Email == newUser.Email);

        return existingUser != null ? existingUser : await service.AddAsync(newUser);
    }

    public ValueTask DeleteAsync(string id)
    {
        User dbVersion = Get(id);
        return authInfo.SSOUserId == dbVersion.Id
            ? service.DeleteAsync(id)
            : throw new SecurityException("Access Denied!");
    }

    public ValueTask<User> UpdateAsync(User entity) =>
        authInfo.SSOUserId == entity.Id
            ? service.UpdateAsync(entity)
            : throw new SecurityException("Access Denied!");

    public async ValueTask<IEnumerable<Result<User>>> AddOrUpdate(
        IEnumerable<User> items
    )
    {
        List<Result<User>> results = [];

        foreach (User item in items)
        {
            try
            {
                bool isAdd = string.IsNullOrWhiteSpace(item.Id);

                results.Add(
                    new Result<User>
                    {
                        Success = true,
                        Item = isAdd ? await AddAsync(item) : await UpdateAsync(item),
                        Message = isAdd ? "Added Successfully" : "Updated Successfully",
                    }
                );
            }
            catch (Exception ex)
            {
                results.Add(
                    new Result<User>
                    {
                        Success = false,
                        Item = item,
                        Message = ex.Message,
                    }
                );
            }
        }

        return results;
    }
    public async ValueTask DeleteAllAsync(IEnumerable<User> items)
    {
        foreach (User item in items)
            await DeleteAsync(item.Id);
    }
}










