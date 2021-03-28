using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserManagementUiDemo.Pages
{
    [Authorize]
    public class TestAuthenticatedModel : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}