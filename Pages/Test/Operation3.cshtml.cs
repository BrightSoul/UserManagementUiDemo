using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages.Test
{
    [Authorize(Policy = nameof(Permission.Operation3))]
    public class Operation3Model : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}