using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;
using UserManagementUiDemo.Models.InputModels;

namespace UserManagementUiDemo.Pages
{
    public class IndexModel : PageModel
    {
        public readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public IndexModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IList<string> Administrators { get; private set; }
        [BindProperty]
        public UserCreateInputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // A scopo dimostrativo, indichiamo nella pagina quali amministratori esistono
            // Vedi il file Index.cshtml: la pagina presenterà un form di creazione di un amministratore se non ne esiste ancora nessuno
            Administrators = await GetAdministrators();
            ViewData["Title"] = "Benvenuto!";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Nota: questa funzionalità esiste solo per agevolare la fruizione di questa demo

            // Il form è stato inviato, creiamo il primo amministratore 
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            // Ma solo se non esiste già un amministratore, altrimenti
            // chiunque potrebbe crearne uno dato che questa pagina
            // è accessibile anche da utenti anonimi
            IList<string> administrators = await GetAdministrators();
            if (administrators.Count > 0)
            {
                ModelState.AddModelError("", "Nel database esiste già almeno un amministratore");
                return await OnGetAsync();
            }

            // Creiamo l'utente
            ApplicationUser user = Input.ToApplicationUser(userManager);
            IdentityResult result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile creare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync();
            }

            // Gli assegniamo il ruolo di amministratore
            Claim administratorRoleClaim = CreateAdministratorRoleClaim();
            result = await userManager.AddClaimAsync(user, administratorRoleClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile creare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync();
            }

            // Emettiamo il cookie di autenticazione per l'utente, così che risulti già loggato
            await signInManager.SignInAsync(user, false);

            ViewData["ConfirmationMessage"] = "Hai creato il primo utente amministratore! Ora puoi gestire gli utenti";
            return RedirectToPage("/Users/Index");
        }

        private async Task<IList<string>> GetAdministrators()
        {
            Claim administratorRoleClaim = CreateAdministratorRoleClaim();
            IList<ApplicationUser> users = await userManager.GetUsersForClaimAsync(administratorRoleClaim);
            return users.Select(user => $"{user.FullName} ({user.Email})").ToList();
        }

        private Claim CreateAdministratorRoleClaim()
        {
            return new(ClaimTypes.Role, nameof(Role.Administrator));
        }
    }
}