using Jobby.Data.entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jobby.Data.context
{
    public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User,IdentityRole<Guid>,Guid>(options)
    {
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationMembers> OrganizationMembers { get; set; }
        public DbSet<OrganizationInvites> OrganizationInvites { get; set; }
        public DbSet<ApplicationNote> ApplicationNotes { get; set; }
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistory { get; set; }
        public DbSet<Interview> Interview { get; set; }
        public DbSet<JobApplications> JobApplications { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Files> Files { get; set; }
        public DbSet<ApplicationFiles> ApplicationFiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
               entity.Property(entity => entity.EnableNotifications).HasDefaultValue(true);

                modelBuilder.HasDefaultSchema("jobby");
            });
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
        
    }
}
