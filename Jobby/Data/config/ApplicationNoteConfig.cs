using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{

    public class ApplicationNoteConfig : IEntityTypeConfiguration<ApplicationNote>
    {
        public void Configure(EntityTypeBuilder<ApplicationNote> b)
        {
            b.Property(x => x.Visibility)
              .HasColumnName("visibility");
            b.ToTable("application_notes", t =>
            {
                t.HasCheckConstraint(
                      "CK_ApplicationNotes_Visibility",
        "visibility IN ('RecruiterOnly', 'CandidateVisible')");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.NoteType)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.Visibility)
                .HasConversion<string>()
                .IsRequired();

            b.Property(x => x.Content)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            b.HasIndex(x => x.ApplicationId);
            b.HasIndex(x => x.CreatedAt);

            b.HasOne(x => x.JobApplications)
                .WithMany(a => a.Notes)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.AuthorUser)
                .WithMany()
                .HasForeignKey(x => x.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
