using AutoMapper;
using Jobby.Data.entities;
using Jobby.Dtos.jobs;

namespace Jobby.Dtos.Profiles
{
    public class JobsProfile : Profile
    {
        public JobsProfile() {
            CreateMap<Jobs, JobsDto>();

            CreateMap<JobsCreateRequestDto, Jobs>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.OrganizationId, opt => opt.Ignore())
                .ForMember(d => d.CreatedByUserId, opt => opt.Ignore())
                .ForMember(d => d.Status, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.PublishedAt, opt => opt.Ignore());

            CreateMap<JobsUpdateRequestDto, Jobs>().ForAllMembers(opt=>opt.Condition((src,dest,srcMember)=>srcMember!=null));

          


        }
    }
}
//public record jobsDto(
//    Guid Id,
//    Guid OrganizationId,
//    string Title,
//    string? Description,
//    string? Location,
//    JobLocationType LocationType,
//    EmploymentType EmploymentType,
//    JobStatus Status,
//    int? SalaryMin,
//    int? SalaryMax,
//    string? Currency,
//    Guid CreatedByUserId,
//    DateTimeOffset? PublishedAt,
//    DateTimeOffset CreatedAt,
//    DateTimeOffset UpdatedAt
//    );
//public record jobsDetailsResponseDto(
//    jobsDto data,
//    int pageLimit,
//    int pageSize,
//    int total
//    );
//public record jobsCreateRequestDto(
//    string Title,
//    string Description,
//    string Location,
//    JobLocationType LocationType,
//    EmploymentType EmploymentType,
//    int SalaryMin,
//    int SalaryMax,
//    string currency
//    );
//public record jobsUpdateRequestDto(
//       string Title,
//    string Description,
//    string Location,
//    JobLocationType LocationType,
//    EmploymentType EmploymentType,
//     JobStatus Status,
//    int SalaryMin,
//    int SalaryMax,
//    string currency
//    );
//}
//public record jobsUpdateStatusRequestDto(
//     JobStatus Status
//    );