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
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            IQueryable<ApplicationUser> userQuery = userManager.Users;
            if (Search is not null and not "")
            {
                userQuery = userQuery.Where(user => EF.Functions.Like(user.FullName, $"%{Search}%") || EF.Functions.Like(user.Email, $"%{Search}%"));
            }
            Users = await userQuery.ToListAsync();
            ViewData["Title"] = $"Elenco utenti ({Users.Count})";
            return Page();
        }
    }
}