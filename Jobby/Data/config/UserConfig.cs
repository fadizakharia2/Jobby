using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class UserConfig:IEntityTypeConfiguration<User>
    {
        public void Configure (EntityTypeBuilder<User> b)
        {
            b.ToTable("users");
            b.HasKey(u => u.Id);
            b.Property(u => u.Email)
                .HasMaxLength(320)
                .IsRequired();
            b.Property(u => u.PasswordHash)
                .IsRequired();
            b.Property(u => u.FirstName)
                .HasMaxLength(100);
                b.Property(u => u.LastName)
                .IsRequired();
            b.Property(u=>u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            b.Property(u => u.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
            b.Property (u => u.UpdatedAt)
                .HasDefaultValueSql(
                    "now()")
                .IsRequired();


        }
    }
}
