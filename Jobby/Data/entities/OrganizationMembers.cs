using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Jobby.Data.entities
{
    public class OrganizationMembers
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        // ADMIN | RECRUITER
        [MaxLength(32)]
        public string Role { get; set; } = default!;

        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
