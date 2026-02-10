using AutoMapper;
using Jobby.Dtos.OrganizationMembersDto;

namespace Jobby.Dtos.Profiles
{
    public class OrganizationInvitesProfile : Profile
    {
        public OrganizationInvitesProfile() { 
           CreateMap<Data.entities.OrganizationInvites, OrganizationInvitesDto>();
           CreateMap<CreateInviteRequestDto, Data.entities.OrganizationInvites>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OrganizationId, o => o.Ignore())
                .ForMember(d => d.TokenHash, o => o.Ignore())
                .ForMember(d => d.ExpiresAt, o => o.Ignore())
                .ForMember(d => d.AcceptedAt, o => o.Ignore())
                .ForMember(d => d.CreatedByUserId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
            CreateMap<AcceptInviteRequestDto, Data.entities.OrganizationInvites>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OrganizationId, o => o.Ignore())
                .ForMember(d => d.Email, o => o.Ignore())
                .ForMember(d => d.InvitedRole, o => o.Ignore())
                .ForMember(d => d.TokenHash, o => o.Ignore())
                .ForMember(d => d.ExpiresAt, o => o.Ignore())
                .ForMember(d => d.CreatedByUserId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
             CreateMap<DeclineInviteRequestDto, Data.entities.OrganizationInvites>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.OrganizationId, o => o.Ignore())
                .ForMember(d => d.Email, o => o.Ignore())
                .ForMember(d => d.InvitedRole, o => o.Ignore())
                .ForMember(d => d.TokenHash, o => o.Ignore())
                .ForMember(d => d.ExpiresAt, o => o.Ignore())
                .ForMember(d => d.CreatedByUserId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore());
        }
    }
}
