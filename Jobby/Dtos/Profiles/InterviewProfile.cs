using AutoMapper;
using Jobby.Data.entities;
using Jobby.Data.enums;
using Jobby.Dtos.Interviews;

namespace Jobby.Dtos.Profiles
{
    public class InterviewProfile : Profile
    {
        public InterviewProfile()
        {
            // 🔹 Entity → Response DTO
            CreateMap<Interview, InterviewDetailResDto>()
                .ForMember(dest => dest.ApplicationId,
                    opt => opt.MapFrom(src => src.ApplicationId))
                .ForMember(dest => dest.ScheduledByUserId,
                    opt => opt.MapFrom(src => src.ScheduledByUserId))
                .ForMember(dest => dest.Stage,
                    opt => opt.MapFrom(src => src.Stage))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StartsAt,
                    opt => opt.MapFrom(src => src.StartsAt))
                .ForMember(dest => dest.EndsAt,
                    opt => opt.MapFrom(src => src.EndsAt))
                .ForMember(dest => dest.Location,
                    opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.MeetingUrl,
                    opt => opt.MapFrom(src => src.MeetingUrl))
                .ForMember(dest => dest.Feedback,
                    opt => opt.MapFrom(src => src.Feedback))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt));

            // 🔹 Create DTO → Entity
            CreateMap<InterviewCreateReqDto, Interview>()
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => InterviewStatus.Scheduled))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(_ => DateTimeOffset.UtcNow));

            // 🔹 Update DTO → Entity
            CreateMap<InterviewUpdateReqDto, Interview>()
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTimeOffset.UtcNow));

            // 🔹 Complete interview
            CreateMap<InterviewCompleteReqDto, Interview>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => InterviewStatus.Completed))
                .ForMember(dest => dest.CompletedAt,
                    opt => opt.MapFrom(_ => DateTimeOffset.UtcNow));

            // 🔹 Cancel interview
            CreateMap<InterviewCancelReqDto, Interview>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(_ => InterviewStatus.Cancelled))
                .ForMember(dest => dest.UpdatedAt,
                    opt => opt.MapFrom(_ => DateTimeOffset.UtcNow))
                .ForMember(dest => dest.CancelReason,
                    opt => opt.MapFrom(src => src.Reason));
        }
    }
}
