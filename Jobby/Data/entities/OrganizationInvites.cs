using System.ComponentModel.DataAnnotations;

namespace Jobby.Data.entities
{
    public class OrganizationInvites
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        [MaxLength(320)]
        public string Email { get; set; } = default!;

        // "Admin" | "Recruiter"
        [MaxLength(32)]
        public string InvitedRole { get; set; } = default!;

        // Store hashed token (not the raw token)
        public string TokenHash { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
