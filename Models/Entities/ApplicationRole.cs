using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserManagementUiDemo.Models.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<IdentityRoleClaim<string>> RoleClaims { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}