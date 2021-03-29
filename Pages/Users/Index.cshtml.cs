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
    public class UserIndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public UserIndexModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IList<ApplicationUser> Users { get; private set; }

        public async Task<IActionResult> OnGetAsync(string search)
        {
            IQueryable<ApplicationUser> userQuery = userManager.Users;
            if (search is not null and not "")
            {
                userQuery = userQuery.Where(user => user.FullName.Contains(search) || user.Email.Contains(search));
            }
            Users = await userQuery.ToListAsync();
            ViewData["Title"] = $"Elenco utenti ({Users.Count})";
            return Page();
        }
    }
}