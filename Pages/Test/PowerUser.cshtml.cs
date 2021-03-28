using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages
{
    [Authorize(Roles = nameof(Role.PowerUser))]
    public class TestPowerUserModel : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}