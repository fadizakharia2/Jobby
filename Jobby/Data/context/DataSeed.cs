using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Data.context
{
    public static class DataSeed
    {
        public static async void seed(this WebApplication app)
        {
            var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            foreach (var r in new[] { "Candidate", "Recruiter", "OrgAdmin" })
                if (!await roleMgr.RoleExistsAsync(r))
                    await roleMgr.CreateAsync(new IdentityRole<Guid>(r));
        }
    }
}
