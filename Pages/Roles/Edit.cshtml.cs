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
    public class RoleEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private const string IndexPage = "/Roles/Index";

        public RoleEditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public RoleEditInfoInputModel RoleInfo { get; private set; }
        public IList<Claim> RoleClaims { get; private set; }
        public IList<string> PermissionNames => GetPermissionNames();
        public IDictionary<string, string> StandardClaimTypes => GetStandardClaimTypes();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            ApplicationRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToPage(IndexPage);
            }
            RoleInfo = RoleEditInfoInputModel.FromApplicationRole(role);

            // Otteniamo anche i claim del ruolo
            RoleClaims = await roleManager.GetClaimsAsync(role);

            ViewData["Title"] = $"Ruolo {role.Name}";
            return Page();
        }

        public async Task<IActionResult> OnPostInfoEditAsync(string id, [Bind(Prefix = nameof(RoleInfo))] RoleEditInfoInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToPage(IndexPage);
            }

            inputModel.CopyToApplicationRole(role);

            IdentityResult result = await roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(RoleInfo), $"Non è stato possibile aggiornare il ruolo. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            ViewData["ConfirmationMessage"] = $"Le informazioni del ruolo {role.Name} sono state aggiornate";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostAssignClaimAsync(string id, EditClaimInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToPage(IndexPage);
            }

            Claim claim = inputModel.ToClaim();

            // Se il claim type è quello dei permessi, verifichiamo che il valore sia tra quelli ammessi
            if (claim.Type == nameof(Permission) && !PermissionNames.Contains(claim.Value))
            {
                ModelState.AddModelError(nameof(RoleClaims), $"Il claim dei permessi ammette solo un valore a scelta tra {string.Join(", ", PermissionNames)}");
                return await OnGetAsync(id);
            }

            // Verifichiamo se questo nuovo claim era già presente nel database
            // Evitiamo duplicati
            IList<Claim> userClaims = await roleManager.GetClaimsAsync(role);
            if (userClaims.Any(userClaim => userClaim.Type == claim.Type && userClaim.Value == claim.Value))
            {
                ModelState.AddModelError(nameof(RoleClaims), $"Il claim '{claim.Type}' con valore '{claim.Value}' è già assegnato al ruolo");
                return await OnGetAsync(id);
            }
            
            IdentityResult result = await roleManager.AddClaimAsync(role, claim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(RoleClaims), $"Non è stato possibile aggiungere il claim '{claim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era un ruolo assegnato al mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(role);

            ViewData["ConfirmationMessage"] = $"Il claim '{claim.Type}' è stato aggiunto con il valore '{claim.Value}'";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostRevokeClaimAsync(string id, [Bind(Prefix = "claim")] EditClaimInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return await OnGetAsync(id);
            }

            ApplicationRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToPage(IndexPage);
            }

            Claim oldClaim = inputModel.ToClaim();
            IdentityResult result = await roleManager.RemoveClaimAsync(role, oldClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(RoleClaims), $"Non è stato possibile rimuovere il claim '{oldClaim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            // Se questo era un ruolo assegnato al mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(role);

            ViewData["ConfirmationMessage"] = $"Il claim '{oldClaim.Type}' con valore '{oldClaim.Value}' è stato rimosso";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            ApplicationRole role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToPage(IndexPage);
            }

            IdentityResult result = await roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(nameof(RoleClaims), $"Non è stato possibile eliminare il ruolo. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era un ruolo assegnato al mio utente, ri-emetto il cookie di autenticazione
            await UpdateIdentityIfNeeded(role);

            ViewData["ConfirmationMessage"] = $"Il ruolo {role.Name} è stato eliminato";
            return RedirectToPage(IndexPage);
        }

        private async Task UpdateIdentityIfNeeded(ApplicationRole role)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            bool hasRole = await userManager.IsInRoleAsync(user, role.Name);
            if (hasRole)
            {
                await signInManager.SignInAsync(user, false);
            }
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

        private IList<string> GetPermissionNames()
        {
            return Enum.GetNames<Permission>();
        }
    }
}