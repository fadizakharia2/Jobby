using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class InterviewConfig : IEntityTypeConfiguration<Interview>
    {
        public void Configure(EntityTypeBuilder<Interview> b)
        {
            b.ToTable("interviews", t =>
            {
                t.HasCheckConstraint(
                    "CK_Interviews_TimeRange",
                "\"StartsAt\" < \"EndsAt\""
                    );
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Stage)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.Location)
                .HasMaxLength(200);

            b.Property(x => x.MeetingUrl)
                .HasMaxLength(500);

            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            b.HasIndex(x => x.ApplicationId);
            b.HasIndex(x => x.StartsAt);

            b.HasOne(x => x.JobApplications)
                .WithMany(a => a.Interviews)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ScheduledByUser)
                .WithMany()
                .HasForeignKey(x => x.ScheduledByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
