// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.AppSecurity.Brokers;
using cCoder.AppSecurity.Brokers.Storages;
using cCoder.AppSecurity.Models;
using DataUserRole = cCoder.Data.Models.Security.UserRole;

namespace cCoder.AppSecurity.Services.Foundations;

internal sealed partial class AccountRoleAssignmentService(
    IRoleBroker roleBroker,
    IUserRoleBroker userRoleBroker)
    : IAccountRoleAssignmentService
{
    public AccountRoleAssignment GetAccountRoleAssignment(
        AccountRoleAssignment accountRoleAssignment) =>
        TryCatch(operation: () =>
        {
            ValidateAccountRoleAssignmentOnGet(
                accountRoleAssignment: accountRoleAssignment);

            accountRoleAssignment.RoleId = roleBroker
                .GetAllRoles(ignoreFilters: true)
                .Where(predicate: role =>
                    role.AppId == accountRoleAssignment.AppId
                    && role.Name == "Users")
                .Select(selector: role => (Guid?)role.Id)
                .FirstOrDefault();

            accountRoleAssignment.IsAssigned = userRoleBroker
                .GetAllUserRoles(ignoreFilters: true)
                .Any(predicate: userRole =>
                    userRole.UserId == accountRoleAssignment.UserId
                    && userRole.RoleId == accountRoleAssignment.RoleId);

            return accountRoleAssignment;
        });

    public ValueTask<AccountRoleAssignment> AddAccountRoleAssignmentAsync(
        AccountRoleAssignment newAccountRoleAssignment) =>
        TryCatch(operation: async ValueTask<AccountRoleAssignment> () =>
        {
            ValidateAccountRoleAssignmentOnAdd(
                newAccountRoleAssignment: newAccountRoleAssignment);

            DataUserRole userRole = new()
            {
                UserId = newAccountRoleAssignment.UserId,
                RoleId = newAccountRoleAssignment.RoleId.Value,
            };

            await userRoleBroker.AddUserRoleAsync(
                entity: userRole);

            return newAccountRoleAssignment;
        });
}