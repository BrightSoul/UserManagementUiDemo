using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;
using UserManagementUiDemo.Models.InputModels;

namespace UserManagementUiDemo.Pages.Roles
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class RoleCreateModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        public RoleCreateModel(RoleManager<ApplicationRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [BindProperty]
        public RoleCreateInputModel Input { get; set; }
        public IActionResult OnGetAsync()
        {
            Input = new RoleCreateInputModel();
            ViewData["Title"] = "Crea nuovo ruolo";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return OnGetAsync();
            }

            ApplicationRole role = Input.ToApplicationRole();
            IdentityResult result = await roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile creare il ruolo. Motivo: {result.Errors.FirstOrDefault().Description}");
                return OnGetAsync();
            }

            ViewData["ConfirmationMessage"] = "Il ruolo è stato creato. Ora puoi modificare i suoi claim";
            return RedirectToPage("/Roles/Edit", new { id = role.Id });
        }
    }
}