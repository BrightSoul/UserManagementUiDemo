using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UserManagementUiDemo.Pages.Test
{
    [AllowAnonymous]
    public class TestAnonymousModel : PageModel
    {
        public IActionResult OnGet()
        {
            ViewData["Title"] = "Autorizzato!";
            return Page();
        }
    }
}