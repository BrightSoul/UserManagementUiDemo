using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels;

public class RoleCreateInputModel
{
    [Required(ErrorMessage = "Il nome è obbligatorio"),
    MaxLength(255, ErrorMessage = "Il nome è troppo lungo"),
    Display(Name = "Nome")]
    public string Name { get; set; }

    public ApplicationRole ToApplicationRole()
    {
        ApplicationRole role = new()
        {
            Name = Name
        };
        return role;
    }
}