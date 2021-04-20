using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages.Users
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class UserIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;

        public UserIndexModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string InRole { get; set; }
        public IEnumerable<SelectListItem> Roles { get; private set; }
        public IList<ApplicationUser> Users { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Roles = roleManager.Roles.Select(role => new SelectListItem(role.Name, role.Name, role.Name == InRole)).ToList();
            IQueryable<ApplicationUser> userQuery = userManager.Users;
            if (Search is not null and not "")
            {
                userQuery = userQuery.Where(user => EF.Functions.Like(user.Email, $"%{Search}%") || EF.Functions.Like(user.FullName, $"%{Search}%"));
            }
            if (InRole is not null and not "")
            {
                userQuery = userQuery.Where(user => user.Roles.Any(role => role.Name == InRole));
            }
            Users = await userQuery.ToListAsync();
            ViewData["Title"] = $"Elenco utenti ({Users.Count})";
            return Page();
        }
    }
}