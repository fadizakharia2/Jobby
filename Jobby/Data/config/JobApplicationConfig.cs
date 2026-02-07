using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{

    public class JobApplicationConfig : IEntityTypeConfiguration<JobApplications>
    {
        public void Configure(EntityTypeBuilder<JobApplications> b)
        {
            b.ToTable("applications");

            b.HasKey(x => x.Id);

            b.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.Source)
                .HasConversion<string>();

            b.Property(x => x.AppliedAt)
                .HasDefaultValueSql("now()");

            b.Property(x => x.LastStatusChangedAt)
                .HasDefaultValueSql("now()");

            b.HasIndex(x => x.JobId);
            b.HasIndex(x => x.CandidateUserId);
            b.HasIndex(x => new { x.OrganizationId, x.Status });

            b.HasIndex(x => new { x.JobId, x.CandidateUserId })
                .IsUnique();

            b.HasOne(x => x.Job)
                .WithMany(j => j.JobApplications)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.CandidateUser)
                .WithMany()
                .HasForeignKey(x => x.CandidateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Organization)
                .WithMany()
                .HasForeignKey(x => x.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
