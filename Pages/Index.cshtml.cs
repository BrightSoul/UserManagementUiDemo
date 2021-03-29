using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;
using UserManagementUiDemo.Models.InputModels;

namespace UserManagementUiDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        public readonly UserManager<ApplicationUser> userManager;
        public readonly RoleManager<IdentityRole> roleManager;
        public IndexModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public IList<string> Managers { get; private set; }
        [BindProperty]
        public UserCreateInputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // A scopo dimostrativo, indichiamo nella pagina quali utenti hanno il permesso di gestione
            // Vedi il file Index.cshtml: la pagina presenterà un form di creazione di utente che ha il permesso di gestione se non ne esiste già uno
            Managers = await GetUsersWithManagementPermission();
            ViewData["Title"] = "Benvenuto!";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Nota: questa funzionalità esiste solo per agevolare la fruizione di questa demo

            // Il form è stato inviato, creiamo il primo utente con permesso di gestione
            if (!ModelState.IsValid)
            {
                return await OnGetAsync();
            }

            // Ma solo se non esiste già un utente con permesso di gestione, altrimenti
            // chiunque potrebbe crearne uno dato che questa pagina
            // è accessibile anche da utenti anonimi
            IList<string> managers = await GetUsersWithManagementPermission();
            if (managers.Count > 0)
            {
                ModelState.AddModelError("", "Nel database esiste già almeno un utente con permesso di gestione");
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

            // Assegniamo il permesso di gestione all'utente
            // Non ha importanza se il claim del permesso è assegnato all'utente
            // Oppure a un ruolo assegnato all'utente
            // In entrambi i casi, il permesso apparirà nella sua ClaimsIdentity
            Claim userManagementPermissionClaim = CreateUserManagementPermissionClaim();
            result = await userManager.AddClaimAsync(user, userManagementPermissionClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile assegnare il permesso di gestione all'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync();
            }

            // Emettiamo il cookie di autenticazione per l'utente, così che risulti già loggato
            await signInManager.SignInAsync(user, false);

            ViewData["ConfirmationMessage"] = "Hai creato il primo utente con permesso di gestione! Ora puoi gestire gli utenti e i ruoli";
            return RedirectToPage("/Users/Index");
        }

        private async Task<IList<string>> GetUsersWithManagementPermission()
        {
            Claim userManagementPermissionClaim = CreateUserManagementPermissionClaim();
            // Estraiamo tutti gli utenti che hanno almeno un ruolo con il permesso di gestione utenti
            IList<ApplicationUser> users = await userManager.Users.Where(user => user.Roles.Any(role => role.RoleClaims.Any(claim => claim.ClaimType == nameof(Permission) && claim.ClaimValue == nameof(Permission.UserManagement)))).ToListAsync();
            return users.Select(user => $"{user.FullName} ({user.Email})").ToList();
        }

        private Claim CreateUserManagementPermissionClaim()
        {
            return new(nameof(Permission), nameof(Permission.UserManagement));
        }
    }
}