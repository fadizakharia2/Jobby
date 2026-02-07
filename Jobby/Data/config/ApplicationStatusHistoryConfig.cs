using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class ApplicationStatusHistoryConfig : IEntityTypeConfiguration<ApplicationStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ApplicationStatusHistory> b)
        {
            b.ToTable("application_status_history");

            b.HasKey(x => x.Id);

            b.Property(x => x.FromStatus)
                .HasConversion<string>();

            b.Property(x => x.ToStatus)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.ChangedAt)
                .HasDefaultValueSql("now()");

            b.HasIndex(x => x.ApplicationId);
            b.HasIndex(x => x.ChangedAt);

            b.HasOne(x => x.JobApplications)
                .WithMany(a => a.StatusHistory)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ChangedByUser)
                .WithMany()
                .HasForeignKey(x => x.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
