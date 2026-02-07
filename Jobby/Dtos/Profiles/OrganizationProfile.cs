using AutoMapper;
using static Jobby.Dtos.OrganizationDtos.OrganizationDtos;
using Jobby.Data.entities;
namespace Jobby.Dtos.Profiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile() {
            CreateMap<Organization, OrganizationDto>();

            // Create request -> entity
            CreateMap<CreateOrganizationRequest, Organization>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedByUserId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.Members, o => o.Ignore())
                .ForMember(d => d.Invites, o => o.Ignore())
                .ForMember(d => d.Jobs, o => o.Ignore());

            // Update request -> entity (map into existing entity)
            CreateMap<UpdateOrganizationRequest, Organization>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedByUserId, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.Members, o => o.Ignore())
                .ForMember(d => d.Invites, o => o.Ignore())
                .ForMember(d => d.Jobs, o => o.Ignore());
        }
    }
}
