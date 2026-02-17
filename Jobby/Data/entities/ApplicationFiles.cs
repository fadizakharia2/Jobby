using Jobby.Data.enums;

namespace Jobby.Data.entities
{
    public class ApplicationFiles
    {
        public Guid Id { get; set; }

        public Guid ApplicationId { get; set; }
        public JobApplications Application { get; set; } = default!;

        public Guid FileId { get; set; }
        public Files File { get; set; } = default!;

        public FilePurpose FilePurpose { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
