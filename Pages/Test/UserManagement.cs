using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages.Test
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class UserManagementModel : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}