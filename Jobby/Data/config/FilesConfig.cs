using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class FilesConfig : IEntityTypeConfiguration<Files>
    {
        public void Configure(EntityTypeBuilder<Files> builder)
        {
            builder.ToTable("file");

            builder.HasIndex(b => b.Id);
            builder.HasIndex(b => b.OwnerUserId);
            builder.HasOne(b => b.OwnerUser)
                .WithMany()
                .HasForeignKey(b=>b.OwnerUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.ContentType);
            builder.Property(b => b.StorageKey).IsRequired(true).HasMaxLength(512);
            builder.Property(b=>b.OriginalName).IsRequired(true).HasMaxLength(255);
            builder.Property(b => b.SizeBytes).IsRequired(true);
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("now()");
        }
    }
}
