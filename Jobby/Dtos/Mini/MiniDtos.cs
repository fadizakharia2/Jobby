using Jobby.Data.enums;

namespace Jobby.Dtos.Mini
{

    public record JobMiniDto(
        Guid Id,
        Guid OrganizationId,
        string Title,
        string? Location,
        JobLocationType LocationType,
        EmploymentType EmploymentType,
        JobStatus Status,
        int? SalaryMin,
        int? SalaryMax,
        string? Currency,
        DateTimeOffset? PublishedAt
    );

    public record CandidateMiniDto(
        Guid Id,
        string Email,
        string FirstName,
        string LastName
    );

    public record ApplicationFileDto(
        Guid FileId,
        string OriginalName,
        string? ContentType,
        long SizeBytes,
        FilePurpose FilePurpose
    );

    public record ApplicationStatusHistoryDto(
        Guid Id,
        ApplicationStatus? FromStatus,
        ApplicationStatus ToStatus,
        Guid ChangedByUserId,
        string? Reason,
        DateTimeOffset ChangedAt
    );

    public record ApplicationNoteDto(
        Guid Id,
        Guid AuthorUserId,
        string NoteType,
        string Visibility,
        string Content,
        DateTimeOffset CreatedAt
    );

    public record InterviewDto(
        Guid Id,
        string Stage,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        string? Location,
        string? MeetingUrl,
        string Status,
        string? Feedback,
        DateTimeOffset CreatedAt
    );
}
