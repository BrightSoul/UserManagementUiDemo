using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels;

public class UserEditProfileInputModel
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

    [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,15}$", ErrorMessage = "La password deve essere lunga tra 8 e 15 caratteri e contenere almeno una maiuscola, una minuscola, un numero e un simbolo"),
    Display(Name = "Reimposta password")]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Le password non corrispondono"),
    Display(Name = "Conferma password")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "Blocca l'account (non potrà fare il login) fino al")]
    public DateTimeOffset? LockoutEnd { get; set; }

    public void CopyToApplicationUser(UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        user.FullName = FullName;
        user.Email = Email;
        user.LockoutEnd = LockoutEnd;
        if (Password is not null and not "")
        {
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, Password);
        }
    }

    public static UserEditProfileInputModel FromApplicationUser(ApplicationUser user)
    {
        return new UserEditProfileInputModel
        {
            FullName = user.FullName,
            Email = user.Email,
            LockoutEnd = user.LockoutEnd
        };
    }
}