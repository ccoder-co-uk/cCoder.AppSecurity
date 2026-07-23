// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.AppSecurity.Models;

public sealed class AccountRoleAssignment
{
    public int AppId { get; set; }
    public string UserId { get; set; }
    public Guid? RoleId { get; set; }
    public bool IsAssigned { get; set; }
}