using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.Mini;

namespace Jobby.Dtos.Application
{
    public record ApplicationDetailsDto(
       List<ApplicationDetailDto> Data,
       int PageLimit,
       int PageNumber,
       int Total
   );

    public record ApplicationDetailDto(
        Guid Id,
        JobMiniDto Job,
        ApplicationStatus Status,
        CandidateMiniDto Candidate,
        ApplicationSource? Source,
        string? CoverLetter,
        List<ApplicationFileDto> Files,
        List<ApplicationStatusHistoryDto> StatusHistory,
        List<ApplicationNoteDto> Notes,
        List<InterviewDto> Interviews,
        DateTimeOffset AppliedAt,
        DateTimeOffset LastStatusChangedAt
    );
    public record ApplicationCreateDto(
        Guid jobId,
        ApplicationSource? Source,
       string? CoverLetter
        );
    public record UpdateApplicationStatusDto(
        ApplicationStatus Status
        );
}
