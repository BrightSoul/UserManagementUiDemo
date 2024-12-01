using Microsoft.AspNetCore.Authorization;

namespace UserManagementUiDemo.Models.Authorization;

// Source: https://codewithmukesh.com/blog/permission-based-authorization-in-aspnet-core
public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; private set; } = permission;
}