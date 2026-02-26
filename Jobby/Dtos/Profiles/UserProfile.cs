using AutoMapper;
using Jobby.Data.entities;
using Jobby.Dtos.Auth;

namespace Jobby.Dtos.Profiles
{
    public class UserProfile:Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDto>();
        }
    }
}
