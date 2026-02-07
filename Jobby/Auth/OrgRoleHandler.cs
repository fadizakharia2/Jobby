using System.Security.Claims;
using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Auth
{
    public class OrgRoleHandler(AppDbContext db) : AuthorizationHandler<OrgRoleRequirement,Guid>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrgRoleRequirement requirement, Guid orgId)
        {
            var userIdStr =  context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return;
            var roles = await db.OrganizationMembers.Where(m=>m.UserId == userId && m.OrganizationId == orgId).Select(m=>m.Role).FirstOrDefaultAsync();
            if (roles == null) return;
            if(requirement.AllowedRoles.Contains(roles))
                context.Succeed(requirement);
        }
    }
}
