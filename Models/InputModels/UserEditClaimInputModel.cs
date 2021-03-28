using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using UserManagementUiDemo.Models.Entities;

namespace UserManagementUiDemo.Models.InputModels
{
    public class UserEditClaimInputModel
    {

        [Required(ErrorMessage = "Il claim type è obbligatorio"),
        MaxLength(255, ErrorMessage = "Il claim type è troppo lungo"),
        Display(Name = "Claim Type")]
        public string ClaimType { get; set; }

        [MaxLength(255, ErrorMessage = "Il claim value è troppo lungo"),
        Display(Name = "Claim Value")]
        public string ClaimValue { get; set; }

        public string OldClaimValue { get; set; }

        public Claim ToClaim()
        {
            return new(ClaimType, ClaimValue);
        }
        public Claim ToOldClaim()
        {
            return new(ClaimType, OldClaimValue);
        }
    }
}