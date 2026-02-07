using System.ComponentModel.DataAnnotations;
using Jobby.Data.enums;
using static System.Net.Mime.MediaTypeNames;

namespace Jobby.Data.entities
{
    public class Interview
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }
        public JobApplications JobApplications { get; set; } = default!;

        public Guid ScheduledByUserId { get; set; }
        public User ScheduledByUser { get; set; } = default!;

        public InterviewStage Stage { get; set; }

        public DateTimeOffset StartsAt { get; set; }
        public DateTimeOffset EndsAt { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(500)]
        public string? MeetingUrl { get; set; }

        public InterviewStatus Status { get; set; }

        public string? Feedback { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
