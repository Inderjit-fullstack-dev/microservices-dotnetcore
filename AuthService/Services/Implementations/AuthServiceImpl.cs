using AuthService.Database;
using AuthService.Entities;
using AuthService.Services.Interfaces;
using AuthService.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Services.Implementations
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthServiceImpl(ApplicationDBContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
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

        public async Task<UserViewModel> Login(LoginViewModel request)
        {
            try
            {
                var user = await _context.Users
                        .Include(x => x.UserRoles).ThenInclude(r => r.Role)
                        .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null)
                {
                    throw new Exception("Account not found!");
                }

                bool isVerified = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

                if (!isVerified)
                {
                    throw new Exception("You have entered wrong password.");
                }

                var mappedUser = _mapper.Map<UserViewModel>(user);
                mappedUser.Token = GenerateToken(user);

                return mappedUser;
            }
            catch (Exception)
            {

                throw;
            }
        }


        private string GenerateToken(User user)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<bool> IsUserExists(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }
    }
}
