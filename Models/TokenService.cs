using Microsoft.IdentityModel.Tokens;
using MyTasks_WebAPI.Models.Domains;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyTasks_WebAPI.Models
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTSettings:IssuerSigningKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = new JwtSecurityToken(
                _configuration["JWTSettings:ValidIssuer"],
                _configuration["JWTSettings:ValidAudience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
