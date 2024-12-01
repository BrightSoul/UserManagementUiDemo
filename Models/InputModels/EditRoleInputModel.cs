using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels;

public class EditRoleInputModel
{

    [Required(ErrorMessage = "Il nome del ruolo è obbligatorio"),
    MaxLength(255, ErrorMessage = "Il nome del ruolo è troppo lungo"),
    Display(Name = "Nome del ruolo")]
    public string Name { get; set; }
}
