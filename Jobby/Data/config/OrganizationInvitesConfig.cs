using System.ComponentModel.DataAnnotations;
using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class OrganizationInvitesConfig : IEntityTypeConfiguration<OrganizationInvites>
    {
        public void Configure(EntityTypeBuilder<OrganizationInvites> b)
        {
            b.ToTable("organization_invites");
            b.HasKey(o => o.Id);
            b.HasOne(o=>o.Organization)
                .WithMany(o=>o.Invites)
                .HasForeignKey(o=>o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
             b.HasOne(o=>o.CreatedByUser)
                .WithMany()
                .HasForeignKey(o=>o.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
             b.Property(o=>o.Email)
                .HasMaxLength(320)
                .IsRequired();
             b.Property(o=>o.InvitedRole)
                .HasMaxLength(32)
                .IsRequired();
             b.Property(o=>o.TokenHash)
                .IsRequired();
             b.Property(o=>o.ExpiresAt)
                .IsRequired();
             b.Property(o=>o.CreatedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
        }
    }
}
