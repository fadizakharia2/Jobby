using Jobby.Data.enums;

namespace Jobby.Data.entities
{
    public class JobApplications
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }
        public Jobs Job { get; set; } = default!;

        public Guid CandidateUserId { get; set; }
        public User CandidateUser { get; set; } = default!;

        public ApplicationStatus Status { get; set; }

        // LinkedIn | Referral | Website | Other
        public ApplicationSource? Source { get; set; }

        public string? CoverLetter { get; set; }

        public DateTimeOffset AppliedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastStatusChangedAt { get; set; } = DateTimeOffset.UtcNow;

        // denormalized for fast org queries
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        public ICollection<ApplicationStatusHistory> StatusHistory { get; set; } = new List<ApplicationStatusHistory>();
        public ICollection<ApplicationNote> Notes { get; set; } = new List<ApplicationNote>();
        public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
    }
}
