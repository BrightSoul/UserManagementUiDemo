using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels;

public class UserCreateInputModel
{
    [Required(ErrorMessage = "Il nome completo è obbligatorio"),
    MaxLength(255, ErrorMessage = "Il nome completo è troppo lungo"),
    Display(Name = "Nome completo")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "L'email è obbligatoria"),
    MaxLength(255, ErrorMessage = "L'email è troppo lunga"),
    EmailAddress(ErrorMessage = "L'indirizzo email non è valido"),
    Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La password è obbligatoria"),
    RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "La password deve essere lunga tra 8 e 15 caratteri e contenere almeno una maiuscola, una minuscola, un numero e un simbolo"),
    Display(Name = "Password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La conferma password è obbligatoria"),
    Compare(nameof(Password), ErrorMessage = "Le password non corrispondono"),
    Display(Name = "Conferma password")]
    public string ConfirmPassword { get; set; }

    public ApplicationUser ToApplicationUser(UserManager<ApplicationUser> userManager)
    {
        ApplicationUser user = new()
        {
            FullName = FullName,
            Email = Email,
            UserName = Email,
            // In questa demo, gli utenti creati dalle pagine di amministrazione si 
            // troveranno già con l'email confermata, così che possano subito fare il login
            EmailConfirmed = true,
            // L'account verrà bloccato automaticamente dopo un certo numero di tentantivi 
            // di login falliti. Puoi configurare i parametri di Lockout (es. numero di 
            // tentantivi, durata del blocco) nella chiamata a AddDefaultIdentity che si 
            // trova nella classe Startup.
            LockoutEnabled = true
        };

        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, Password);
        return user;
    }
}