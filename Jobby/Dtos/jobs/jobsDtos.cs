using Jobby.Data.enums;

namespace Jobby.Dtos.jobs
{
    public record JobsDto(
        Guid Id,
        Guid OrganizationId,
        string Title,
        string? Description,
        string? Location,
        JobLocationType LocationType,
        EmploymentType EmploymentType,
        JobStatus Status,
        int? SalaryMin,
        int? SalaryMax,
        string? Currency,
        Guid CreatedByUserId,
        DateTimeOffset? PublishedAt,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
        );
    public record JobsDetailsResponseDto(
        List<JobsDto> Data,
        int PageLimit,
        int PageNumber,
        int Total
        );
    public record JobsCreateRequestDto(
        string Title,
        string? Description,
        string? Location,
        JobLocationType LocationType,
        EmploymentType EmploymentType,
        int? SalaryMin,
        int? SalaryMax,
        string? Currency
        );
    public record JobsUpdateRequestDto(
           string? Title,
        string? Description,
        string? Location,
        JobLocationType? LocationType,
        EmploymentType? EmploymentType,
         JobStatus? Status,
        int? SalaryMin,
        int? SalaryMax,
        string? Currency
        );
}
