using System.ComponentModel.DataAnnotations;
using Jobby.Data.enums;
using static System.Net.Mime.MediaTypeNames;

namespace Jobby.Data.entities
{
    public class Interview
    {
        public Guid Id { get; set; }

        // 🔹 Relations
        public Guid ApplicationId { get; set; }
        public JobApplications JobApplications { get; set; } = default!;

        public Guid ScheduledByUserId { get; set; }
        public User ScheduledByUser { get; set; } = default!;

        // 🔹 Interview details
        public InterviewStage Stage { get; set; }
        public InterviewType Type { get; set; }
        public InterviewStatus Status { get; set; }

        public DateTimeOffset StartsAt { get; set; }
        public DateTimeOffset EndsAt { get; set; }
        public int DurationMinutes { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(500)]
        public string? MeetingUrl { get; set; }

        // 🔹 Outcome
        public string? Feedback { get; set; }

        [MaxLength(500)]
        public string? CancelReason { get; set; }

        // 🔹 Audit
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
    }
}
