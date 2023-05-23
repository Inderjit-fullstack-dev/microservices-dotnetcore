using AuthService.Database;
using AuthService.Entities;
using AuthService.Services.Interfaces;
using AuthService.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services.Implementations
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public AuthServiceImpl(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserViewModel>> GetUsers()
        {
            try
            {
                var users = await _context.Users.Include(x => x.UserRoles)
                    .ThenInclude(r => r.Role)
                    .Select(u => new UserViewModel
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Email = u.Email,
                        PhoneNumber = u.PhoneNumber,
                        Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                    }).ToListAsync();

                return users;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> CreateUser(SignupViewModel request)
        {
            try
            {
                if (await IsUserExists(request.Email))
                {
                    throw new Exception("User with this email already exisits.");
                }

                var roleInDb = await _context.Roles.FirstOrDefaultAsync(x => x.Name.ToLower() == request.Role.ToLower());

                if (roleInDb == null)
                {
                    throw new Exception("Invalid role!");
                }

                var user = _mapper.Map<User>(request);
                user.CreatedDate = DateTime.Now;

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                UserRole userRole = new()
                {
                    User = user,
                    Role = roleInDb
                };

                using var transaction = _context.Database.BeginTransaction();
                try
                {
                    _context.Users.Add(user);
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<bool> IsUserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<UserViewModel> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        
    }
}
