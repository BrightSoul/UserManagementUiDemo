using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages.Users
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class RoleIndexModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> roleManager;

        public RoleIndexModel(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        public IList<ApplicationRole> Roles { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = await roleManager.Roles.ToListAsync();
            ViewData["Title"] = $"Elenco ruoli ({Roles.Count})";
            return Page();
        }
    }
}