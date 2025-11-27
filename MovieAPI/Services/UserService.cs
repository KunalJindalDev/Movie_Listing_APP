using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MovieApp.Models.DBModels;
using MovieApp.Models.RequestModels;
using MovieApp.Models.ResponseModels;
using MovieApp.Repositories.Interfaces;
using MovieApp.Services.Interfaces;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpirationHours;

        public UserService(IUserReadRepository userReadRepository, IUserWriteRepository userWriteRepository, IConfiguration configuration)
        {
            _userReadRepository = userReadRepository;
            _userWriteRepository = userWriteRepository;
            _jwtSecret = configuration["Jwt:Key"]; 
            _jwtIssuer = configuration["Jwt:Issuer"];
            _jwtAudience = configuration["Jwt:Audience"];
            _jwtExpirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "1"); 
        }

        public bool Signup(SignupRequest request)
        {
            try
            {
                var mailAddress = new MailAddress(request.Email); 
            }
            catch (FormatException)
            {
                return false; 
            }

            if (_userReadRepository.GetByEmail(request.Email) != null)
            {
                return false; 
            }

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 6)
            {
                return false; 
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword
            };

            _userWriteRepository.Create(user);
            return true;
        }

        public LoginResponse Login(LoginRequest request)
        {
            var user = _userReadRepository.GetByEmail(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null; 
            }

            // Generate JWT token for authenticated user
            var token = GenerateJwtToken(user);
            return new LoginResponse { AccessToken = token };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtExpirationHours), 
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
