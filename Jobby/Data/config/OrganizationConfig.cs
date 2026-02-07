using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class OrganizationConfig : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> b)
        {
            b.ToTable("organizations");
            b.HasKey(o => o.Id);
            b.Property(o => o.Slug)
                .IsRequired()
                .HasMaxLength(200);
            b.HasIndex(o => o.Slug)
                .IsUnique();
            b.Property(b=>b.Name)
                .HasMaxLength(200);
            b.Property(o => o.CreatedAt)
                .HasDefaultValueSql("now()");
            b.Property(o => o.UpdatedAt)
                .HasDefaultValueSql("now()");
            b.HasMany(o => o.Jobs)
                .WithOne(x => x.Organization)
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(o=>o.Members)
                .WithOne(o=>o.Organization)
                .HasForeignKey(o=>o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(o=>o.Invites)
                .WithOne(o=>o.Organization)
                .HasForeignKey(o=>o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(o=>o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o=>o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
