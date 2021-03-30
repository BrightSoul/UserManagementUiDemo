using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagementUiDemo.Models.Entities;
using UserManagementUiDemo.Models.Enums;
using UserManagementUiDemo.Models.InputModels;

namespace UserManagementUiDemo.Pages.Users
{
    [Authorize(Policy = nameof(Permission.UserManagement))]
    public class UserEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private const string IndexPage = "/Users/Index";

        public UserEditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public UserEditProfileInputModel UserProfile { get; private set; }
        public IList<Claim> UserClaims { get; private set; }
        public IList<string> UserRoles { get; private set; }
        public IList<ApplicationRole> Roles { get; private set; }
        public IDictionary<string, string> StandardClaimTypes => GetStandardClaimTypes();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }
            UserProfile = UserEditProfileInputModel.FromApplicationUser(user);

            // Otteniamo i nomi di tutti i ruoli disponibili
            Roles = await roleManager.Roles.ToListAsync();

            // Otteniamo i ruoli dell'utente
            UserRoles = await userManager.GetRolesAsync(user);

            // Otteniamo anche i claim dell'uente
            UserClaims = await userManager.GetClaimsAsync(user);

            ViewData["Title"] = $"Utente {user.FullName}";
            return Page();
        }

        public async Task<IActionResult> OnPostProfileEditAsync(string id, [Bind(Prefix = nameof(UserProfile))] UserEditProfileInputModel profileInputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            profileInputModel.CopyToApplicationUser(userManager, user);

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserProfile), $"Non è stato possibile aggiornare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            ViewData["ConfirmationMessage"] = $"Il profilo dell'utente {user.FullName} è stato aggiornato";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostAssignRoleAsync(string id, EditRoleInputModel roleInputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            // Verifichiamo se questo ruolo esiste nel database
            ApplicationRole role = await roleManager.FindByNameAsync(roleInputModel.Name);
            if (role == null)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Il ruolo '{roleInputModel.Name}' non esiste nel database");
                return await OnGetAsync(id);
            }

            // Verifichiamo se questo nuovo ruolo era già assegnato all'utente
            // Evitiamo duplicati
            IList<string> userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Contains(role.Name))
            {
                ModelState.AddModelError(nameof(UserClaims), $"Il ruolo '{role.Name}' è già assegnato all'utente");
                return await OnGetAsync(id);
            }
            
            IdentityResult result = await userManager.AddToRoleAsync(user, roleInputModel.Name);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Non è stato possibile assegnare il ruolo '{roleInputModel.Name}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era il mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(user);

            ViewData["ConfirmationMessage"] = $"Il ruolo '{roleInputModel.Name}' è stato assegnato all'utente";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostRevokeRoleAsync(string id, [Bind(Prefix = "role")] EditRoleInputModel roleInputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            IdentityResult result = await userManager.RemoveFromRoleAsync(user, roleInputModel.Name);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Non è stato possibile revocare il ruolo '{roleInputModel.Name}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            // Se questo era il mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(user);

            ViewData["ConfirmationMessage"] = $"Il ruolo'{roleInputModel.Name}' è stato revocato";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostAssignClaimAsync(string id, EditClaimInputModel claimInputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            Claim claim = claimInputModel.ToClaim();

            // Verifichiamo se questo nuovo claim era già assegnato all'utente
            // Evitiamo duplicati
            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            if (userClaims.Any(userClaim => userClaim.Type == claim.Type && userClaim.Value == claim.Value))
            {
                ModelState.AddModelError(nameof(UserClaims), $"Il claim '{claim.Type}' con valore '{claim.Value}' è già assegnato all'utente");
                return await OnGetAsync(id);
            }
            
            IdentityResult result = await userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Non è stato possibile assegnare il claim '{claim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era il mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(user);

            ViewData["ConfirmationMessage"] = $"Il claim '{claim.Type}' è stato assegnato con il valore '{claim.Value}'";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostRevokeClaimAsync(string id, [Bind(Prefix = "claim")] EditClaimInputModel claimInputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            Claim oldClaim = claimInputModel.ToClaim();
            IdentityResult result = await userManager.RemoveClaimAsync(user, oldClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Non è stato possibile rimuovere il claim '{oldClaim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            // Se questo era il mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(user);

            ViewData["ConfirmationMessage"] = $"Il claim '{oldClaim.Type}' con valore '{oldClaim.Value}' è stato rimosso";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }

            IdentityResult result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(UserClaims), $"Non è stato possibile eliminare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era l'utente con il quale avevo fatto il login, allora eseguo anche il logout
            if (user.Id == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                await signInManager.SignOutAsync();
                ViewData["ConfirmationMessage"] = $"Il tuo utente è stato eliminato ed è stato fatto il logout automaticamente.";
                return RedirectToPage("/Index");
            }

            ViewData["ConfirmationMessage"] = $"L'utente {user.FullName} è stato eliminato";
            return RedirectToPage(IndexPage);
        }

        private async Task UpdateIdentityIfNeeded(ApplicationUser user)
        {
            if (user.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return;
            }

            await signInManager.SignInAsync(user, false);
        }

        private IDictionary<string, string> GetStandardClaimTypes()
        {
            // Usiamo la reflection per ottenere l'elenco dei valori
            // presenti nella classe ClaimTypes.
            // Verranno mostrati nella view in un menu a tendina,
            // per facilitare la selezione
            return typeof(ClaimTypes)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .OrderBy(field => field.Name)
                .ToDictionary(field => field.GetValue(null) as string, field => field.Name);
        }
    }
}