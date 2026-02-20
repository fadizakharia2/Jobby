using Jobby.Data.enums;

namespace Jobby.Dtos.Interviews
{
    public record InterviewDetailsResDto(
        List<InterviewDetailResDto> Data,
        int PageNumber,
        int PageLimit,
        int Total
    );

    public record InterviewDetailResDto(
        Guid Id,
        Guid ApplicationId,
        Guid ScheduledByUserId,
        InterviewStage Stage,
        InterviewStatus Status,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        string? Location,
        string? MeetingUrl,
        string? Feedback,
        DateTimeOffset CreatedAt
    );

    public record InterviewCreateReqDto(
        Guid ApplicationId,
        InterviewStage Stage,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        string? Location,
        string? MeetingUrl
    );

    public record InterviewUpdateReqDto(
        InterviewStage Stage,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        string? Location,
        string? MeetingUrl
    );
    public record InterviewCompleteReqDto(
        string? Feedback
    );

    public record InterviewCancelReqDto(
        string Reason
    );
}
