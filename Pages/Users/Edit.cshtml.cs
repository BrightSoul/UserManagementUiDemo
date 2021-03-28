using System;
using System.Collections.Generic;
using System.Linq;
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
    [Authorize(Roles = nameof(Role.Administrator))]
    public class UserEditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private const string IndexPage = "/Users/Index";
        private readonly SignInManager<ApplicationUser> signInManager;

        public UserEditModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        public ApplicationUser Profile { get; private set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToPage(IndexPage);
            }
            Profile = user;

            ViewData["Title"] = user.FullName;
            return Page();
        }

        public async Task<IActionResult> OnPostProfileEditAsync(string id, UserEditProfileInputModel inputModel)
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

            inputModel.CopyToApplicationUser(userManager, user);

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile aggiornare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            return RedirectToPage(IndexPage);
        }

        public async Task<IActionResult> OnPostAssignClaimAsync(string id, UserEditClaimInputModel inputModel)
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

            Claim claim = inputModel.ToClaim();
            IdentityResult result = await userManager.AddClaimAsync(user, claim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile aggiungere il claim '{claim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            ViewData["ConfirmationMessage"] = $"Il claim '{claim.Type}' è stato aggiunto con il valore '{claim.Value}'";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostUpdateClaimAsync(string id, UserEditClaimInputModel inputModel)
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

            Claim newClaim = inputModel.ToClaim();
            Claim oldClaim = inputModel.ToOldClaim();
            IdentityResult result = await userManager.ReplaceClaimAsync(user, oldClaim, newClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile aggiornare il claim '{newClaim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

            ViewData["ConfirmationMessage"] = $"Il claim '{newClaim.Type}' è stato aggiornato dal valore '{oldClaim.Value}' al valore '{newClaim.Value}'";
            return await OnGetAsync(id);
        }

        public async Task<IActionResult> OnPostRemoveClaimAsync(string id, UserEditClaimInputModel inputModel)
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

            Claim oldClaim = inputModel.ToOldClaim();
            IdentityResult result = await userManager.RemoveClaimAsync(user, oldClaim);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", $"Non è stato possibile rimuovere il claim '{oldClaim.Type}'. Motivo: {result.Errors.FirstOrDefault()?.Description}");
            }

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
                ModelState.AddModelError("", $"Non è stato possibile eliminare l'utente. Motivo: {result.Errors.FirstOrDefault()?.Description}");
                return await OnGetAsync(id);
            }

            // Se questo era l'utente con il quale avevo fatto il login, allora eseguo anche il logout
            if (user.Id == User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                await signInManager.SignOutAsync();
                ViewData["ConfirmationMessage"] = $"Il tuo utente è stato eliminato ed è stato fatto il logout automaticamente.";
                return RedirectToPage("/Index");
            }

            ViewData["ConfirmationMessage"] = $"L'utente {user.FullName} è stato eliminato";
            return RedirectToPage(IndexPage);
        }
    }
}