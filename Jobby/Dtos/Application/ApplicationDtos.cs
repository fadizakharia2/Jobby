using Jobby.Data.entities;
using Jobby.Data.enums;

namespace Jobby.Dtos.Application
{
    public record ApplicationDetailsDto
    (
        List<ApplicationDetailDto> Data,
        int PageLimit,
        int PageNumber,
        int Total
        );
    
    public record ApplicationDetailDto(
        Jobs Job,
        ApplicationStatus Status,
        User CandidateUser,
        ApplicationSource? Source,
       string? CoverLetter,
       List<ApplicationFiles> ApplicationFiles,
       List<ApplicationStatusHistory> ApplicationStatusHistory,
       List<ApplicationNote> ApplicationNotes,
       List<Interview> Interviews,
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
