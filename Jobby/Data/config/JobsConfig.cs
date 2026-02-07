using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config;

public class JobsConfig : IEntityTypeConfiguration<Jobs>
{
    public void Configure(EntityTypeBuilder<Jobs> b)
    {
        b.ToTable("jobs", t =>
        {
            t.HasCheckConstraint(
          "CK_Jobs_SalaryRange",
          "\"SalaryMin\" IS NULL OR \"SalaryMax\" IS NULL OR \"SalaryMin\" <= \"SalaryMax\""
      );
        });

        b.HasKey(x => x.Id);

        b.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.Location)
            .HasMaxLength(200);

        b.Property(x => x.Currency)
            .HasMaxLength(10);

        // Enums → string
        b.Property(x => x.LocationType)
            .HasConversion<string>()
            .IsRequired();

        b.Property(x => x.EmploymentType)
            .HasConversion<string>()
            .IsRequired();

        b.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("now()");

        b.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("now()");

        b.HasIndex(x => x.OrganizationId);
        b.HasIndex(x => x.Status);

        b.HasOne(x => x.Organization)
            .WithMany(o => o.Jobs)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.JobApplications)
            .WithOne(a => a.Job)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}