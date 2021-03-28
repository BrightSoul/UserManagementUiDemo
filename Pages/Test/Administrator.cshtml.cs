using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Pages
{
    [Authorize(Roles = nameof(Role.Administrator))]
    public class AdministratorModel : PageModel
    {
    }
}