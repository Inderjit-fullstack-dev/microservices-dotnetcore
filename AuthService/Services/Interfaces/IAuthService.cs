using AuthService.Entities;
using AuthService.ViewModels;

namespace AuthService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<List<UserViewModel>> GetUsers();
        Task<bool> CreateUser(SignupViewModel request);
        Task<UserViewModel> Login(LoginViewModel request);
    }
}
