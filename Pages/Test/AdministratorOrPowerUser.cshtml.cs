using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages
{
    [Authorize(Roles = nameof(Role.Administrator) + "," + nameof(Role.PowerUser))]
    public class AdministratorOrPowerUserModel : PageModel
    {
    }
}