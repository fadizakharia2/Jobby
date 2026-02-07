using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> b)
        {
            b.ToTable("refresh_tokens");

            b.HasKey(x => x.Id);

            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(256);

            b.Property(x => x.ExpiresAt)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            b.Property(x => x.RevokedAt);

            b.Ignore(x => x.IsActive);

            b.HasIndex(x => x.TokenHash)
                .IsUnique();
        }
    }
}