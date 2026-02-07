using System.ComponentModel.DataAnnotations;
using Jobby.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class OrganizationMembersConfig : IEntityTypeConfiguration<OrganizationMembers>
    {
        public void Configure(EntityTypeBuilder<OrganizationMembers> b)
        {
            b.ToTable("members");
            b.HasKey(o => o.Id);
            b.Property(o => o.Role)
                .IsRequired()
                .HasMaxLength(32);
             b.HasOne(o=>o.Organization)
                .WithMany(o=>o.Members)
                .HasForeignKey(o=>o.OrganizationId)
                .OnDelete(DeleteBehavior.Cascade);
             b.HasOne(o=>o.User)
                .WithMany()
                .HasForeignKey(o=>o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
             b.Property(o=>o.JoinedAt)
                .HasDefaultValueSql("now()")
                .IsRequired();
        }
    }
}
