using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Auth
{
    public class Auth : IAuth
    {
        private readonly IConfiguration configuration;
        private readonly IUserRepository userRepository;
        private readonly IAdminRepository adminRepository;

        public Auth(IConfiguration configuration, IUserRepository userRepository, IAdminRepository adminRepository)
        {
            this.configuration = configuration;
            this.userRepository = userRepository;
            this.adminRepository = adminRepository;
        }

        public bool AuthenticateUser(LoginDto loginDto)
        {
            if (userRepository.UserWithCredentialsExists(loginDto.Email, loginDto.Password))
            {
                return true;
            }

            if (adminRepository.AdminWithCredentialsExists(loginDto.Email, loginDto.Password))
            {
                return true;
            }

            return false;
        }

        public bool IsAdmin(LoginDto loginDto)
        {
            if (adminRepository.AdminWithCredentialsExists(loginDto.Email, loginDto.Password))
            {
                return true;
            }

            return false;
        }

        
        public string GenerateJwt(LoginDto loginDto)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            string role = IsAdmin(loginDto) ? "Admin" : "User";

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, loginDto.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                                             configuration["Jwt:Issuer"],
                                             claims,
                                             expires: DateTime.Now.AddMinutes(120),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
