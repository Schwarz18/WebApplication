using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserProfileDto>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserProfileDto>().ReverseMap();
            CreateMap<ApplicationUser, ChangePasswordDto>().ReverseMap();

        }
    }
}
