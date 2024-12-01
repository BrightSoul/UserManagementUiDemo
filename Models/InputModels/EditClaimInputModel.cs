using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace UserManagementUiDemo.Models.InputModels;

public class EditClaimInputModel
{

    [Required(ErrorMessage = "Il claim type è obbligatorio"),
    MaxLength(255, ErrorMessage = "Il claim type è troppo lungo"),
    Display(Name = "Claim Type")]
    public string Type { get; set; }

    [MaxLength(255, ErrorMessage = "Il claim value è troppo lungo"),
    Display(Name = "Claim Value")]
    public string Value { get; set; }

    public Claim ToClaim()
    {
        return new(Type, Value ?? string.Empty);
    }
}
