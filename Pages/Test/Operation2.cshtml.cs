using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages.Test
{
    [Authorize(Policy = nameof(Permission.Operation2))]
    public class Operation2Model : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}