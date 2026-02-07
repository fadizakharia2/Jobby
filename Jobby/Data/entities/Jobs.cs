using System.ComponentModel.DataAnnotations;
using Jobby.Data.enums;
namespace Jobby.Data.entities
{
    public class Jobs
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; } = default!;

        [MaxLength(200)]
        public string Title { get; set; } = default!;

        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        // Remote | Hybrid | Onsite
        public JobLocationType LocationType { get; set; }

        // FullTime | PartTime | Contract | Internship
        public EmploymentType EmploymentType { get; set; }

        // Draft | Published | Closed
        public JobStatus Status { get; set; }

        public int? SalaryMin { get; set; }
        public int? SalaryMax { get; set; }

        [MaxLength(10)]
        public string? Currency { get; set; }

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        public DateTimeOffset? PublishedAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<JobApplications> JobApplications { get; set; } = new List<JobApplications>();
    }
}
