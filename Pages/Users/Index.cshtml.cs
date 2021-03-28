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
    [Authorize(Roles = nameof(Role.Administrator))]
    public class UserIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserIndexModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IList<ApplicationUser> Users { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int userCount = await userManager.Users.CountAsync();
            ViewData["Title"] = $"Elenco utenti ({userCount})";
            Users = await userManager.Users.ToListAsync();
            return Page();
        }
    }
}