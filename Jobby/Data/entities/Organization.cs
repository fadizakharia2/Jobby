using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Jobby.Data.entities
{
    public class Organization
    {
        public Guid Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<OrganizationMembers> Members { get; set; } = new List<OrganizationMembers>();
        public ICollection<OrganizationInvites> Invites { get; set; } = new List<OrganizationInvites>();
        public ICollection<Jobs> Jobs { get; set; } = new List<Jobs>();
    }
}
