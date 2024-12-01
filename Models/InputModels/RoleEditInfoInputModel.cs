using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels;

public class RoleEditInfoInputModel
{
    [Required(ErrorMessage = "Il nome è obbligatorio"),
    MaxLength(255, ErrorMessage = "Il nome è troppo lungo"),
    Display(Name = "Nome completo")]
    public string Name { get; set; }

    [Display(Name = "Descrizione (opzionale)"),
    MaxLength(255, ErrorMessage = "La descrizione è troppo lunga")]
    public string Description { get; set; }
    public void CopyToApplicationRole(ApplicationRole role)
    {
        role.Name = Name;
        role.Description = Description;
    }

    public static RoleEditInfoInputModel FromApplicationRole(ApplicationRole role)
    {
        return new RoleEditInfoInputModel
        {
            Name = role.Name,
            Description = role.Description
        };
    }
}