using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;
using UserManagementUiDemo.Models.InputModels;

namespace UserManagementUiDemo.Pages.Users
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class UserCreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        public UserCreateModel(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public UserCreateInputModel Input { get; set; }
        public IActionResult OnGetAsync()
        {
            Input = new UserCreateInputModel();
            ViewData["Title"] = "Crea nuovo utente";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGetAsync();
            }

            ApplicationUser user = Input.ToApplicationUser(userManager);
            IdentityResult result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile creare l'utente. Motivo: {result.Errors.FirstOrDefault().Description}");
                return OnGetAsync();
            }

            ViewData["ConfirmationMessage"] = "L'utente è stato creato. Ora puoi modificare il suo profilo e i suoi claim";
            return RedirectToPage("/Users/Edit", new { id = user.Id });
        }
    }
}