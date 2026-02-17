using Jobby.Data.entities;
using Jobby.Data.enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobby.Data.config
{
    public class ApplicationFilesConfig : IEntityTypeConfiguration<ApplicationFiles>
    {
        public void Configure(EntityTypeBuilder<ApplicationFiles> builder)
        {
            builder.ToTable("application_files");
            builder.HasIndex(x=> new {x.ApplicationId,x.FileId});
            builder.HasKey(x=>x.FileId);
            builder.HasOne(x => x.Application)
                .WithMany(b => b.ApplicationFiles)
                .HasForeignKey(x => x.ApplicationId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.File)
                .WithMany()
                .HasForeignKey(x => x.FileId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Property(x => x.FilePurpose);
            builder.Property(x => x.CreatedAt)
                .HasDefaultValueSql("now()");
            builder.Property(x => x.UpdatedAt)
                .HasDefaultValueSql("now()");
        }
    }
}
//using Jobby.Data.enums;

//namespace Jobby.Data.entities
//{
//    public class ApplicationFiles
//    {
//        public Guid Id { get; set; }

//        public Guid ApplicationId { get; set; }
//        public JobApplications Application { get; set; } = default!;

//        public Guid FileId { get; set; }
//        public Files File { get; set; } = default!;

//        public FilePurpose FilePurpose { get; set; }

//        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
//        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
//    }
//}
