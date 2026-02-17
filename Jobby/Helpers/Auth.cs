
using System.Security.Claims;
using System.Security.Principal;
using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Helpers
{
    public static class Auth
    {
        public static async Task<bool> CanAccessApplicationAsync (Guid applicationId,IAuthorizationService auth,ClaimsPrincipal principal ,AppDbContext db, CancellationToken ct)
        {
            
            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var jobApplicationFound =  await db.JobApplications.AsNoTracking().FirstOrDefaultAsync(a=>a.Id == applicationId,ct);
            if(!Guid.TryParse(userIdStr,out var userId))
            {
                return false;
            }
           if (jobApplicationFound is null)
            {
                return false;
            }
            if (jobApplicationFound.CandidateUserId == userId) 
            {
                return true;
            }
          var orgAuthResult =  await auth.AuthorizeAsync(principal, jobApplicationFound.OrganizationId, "OrgRecruiter");
            if (orgAuthResult.Succeeded)
            {
                return true;
            }
            return false;

        }
    }
}
