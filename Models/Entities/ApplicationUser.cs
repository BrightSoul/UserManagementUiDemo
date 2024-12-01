using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace UserManagementUiDemo.Models.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public ICollection<IdentityUserClaim<string>> UserClaims { get; set; }
    public ICollection<ApplicationRole> Roles { get; set; }
}