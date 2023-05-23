using AuthService.Entities;
using AuthService.ViewModels;
using AutoMapper;

namespace AuthService
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<SignupViewModel, User>();
        }
    }
}
