using System.Numerics;

namespace Jobby.Data.entities
{
    public class Files
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public User OwnerUser { get; set; } = default!;
        public string StorageKey { get; set; } = "";
        public string OriginalName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long SizeBytes { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
