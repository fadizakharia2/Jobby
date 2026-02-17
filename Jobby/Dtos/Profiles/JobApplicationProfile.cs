using AutoMapper;
using Jobby.Data.entities;
using Jobby.Dtos.Application;

namespace Jobby.Dtos.Profiles
{
    public class JobApplicationProfile:Profile
    {
        public JobApplicationProfile() {
            CreateMap<JobApplications, ApplicationDetailDto>();
       }
    }
}
