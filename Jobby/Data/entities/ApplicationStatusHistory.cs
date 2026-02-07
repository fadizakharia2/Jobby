using Jobby.Data.enums;
using static System.Net.Mime.MediaTypeNames;

namespace Jobby.Data.entities
{
    public class ApplicationStatusHistory
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }
        public JobApplications JobApplications { get; set; } = default!;

        public ApplicationStatus? FromStatus { get; set; }
        public ApplicationStatus ToStatus { get; set; }

        public Guid ChangedByUserId { get; set; }
        public User ChangedByUser { get; set; } = default!;

        public string? Reason { get; set; }

        public DateTimeOffset ChangedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
