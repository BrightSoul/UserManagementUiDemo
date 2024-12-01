using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using UserManagementUiDemo.Models.Enums;

namespace UserManagementUiDemo.Models.Authorization;

// Source: https://codewithmukesh.com/blog/permission-based-authorization-in-aspnet-core
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User?.Identity.IsAuthenticated != true)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var hasPermission = context.User.HasClaim(nameof(Permission), requirement.Permission);
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}