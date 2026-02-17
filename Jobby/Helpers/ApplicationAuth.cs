using System.Security.Claims;
using Jobby.Data.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Helpers
{
    public static class ApplicationAuth
    {
        public static async Task<bool> CanAccessApplication(Guid applicationId, IAuthorizationService auth, ClaimsPrincipal principal, AppDbContext db, CancellationToken ct)
        {
            var UserIdParseResult =Guid.TryParse(ClaimTypes.NameIdentifier, out var userId);

            if(UserIdParseResult == false)
            {
                return false;
            }
          var foundUser =  await db.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);

            if (foundUser == null)
            {
                return false;
            }
            var foundApplication = await db.JobApplications.FirstOrDefaultAsync(x => x.Id == applicationId,ct);
            if (foundApplication == null) {
                return false;
            }
            if(foundApplication.CandidateUserId == userId)
            {
                return true;
            }

          var orgResult = await  auth.AuthorizeAsync(principal, foundApplication.OrganizationId, "OrgRecruiter");
            if (orgResult.Succeeded)
            {
                return true;
            }
            return false;
        }
    }
}
