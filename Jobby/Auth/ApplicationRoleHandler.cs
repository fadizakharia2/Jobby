using System.Security.Claims;
using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Auth
{
    public class ApplicationRoleHandler(AppDbContext db) : AuthorizationHandler<ApplicationRoleRequirement, Dictionary<string, Guid>>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApplicationRoleRequirement requirement, Dictionary<string,Guid> idsDict)
        {
            var user = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!idsDict.TryGetValue("applicationId", out var applicationId)) return;
            if(!idsDict.TryGetValue("organizationId", out var organizationId)) return;
            if (user == null)
            {
                return;
            }
            if (!Guid.TryParse(user, out var userId))
                return;
            var foundApplication =await db.JobApplications.FirstOrDefaultAsync(x=>x.Id == applicationId);
            if (foundApplication == null)
            {
                return;
            }
            if(foundApplication.CandidateUserId == userId)
            {
               context.Succeed(requirement);
            }
            var roles = await db.OrganizationMembers.Where(m => m.UserId == userId && m.OrganizationId == organizationId).Select(m => m.Role).FirstOrDefaultAsync();
            if (roles == null) return;
            if (requirement.AllowedRoles.Contains(roles))
                context.Succeed(requirement);
        }
    }
}
