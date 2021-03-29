using Microsoft.AspNetCore.Authorization;

namespace UserManagementUiDemo.Models.Authorization
{
    // Source: https://codewithmukesh.com/blog/permission-based-authorization-in-aspnet-core
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
        
        public string Permission { get; private set; }
    }
}