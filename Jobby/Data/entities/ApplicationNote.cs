using Jobby.Data.enums;
using static System.Net.Mime.MediaTypeNames;

namespace Jobby.Data.entities
{

    public class ApplicationNote
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }
        public JobApplications JobApplications { get; set; } = default!;

        public Guid AuthorUserId { get; set; }
        public User AuthorUser { get; set; } = default!;

        public NoteType NoteType { get; set; }
        public NoteVisibility Visibility { get; set; }

        public string Content { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
