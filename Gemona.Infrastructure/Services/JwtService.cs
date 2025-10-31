using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expireDays;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer não configurado");
            _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience não configurado");
            _expireDays = int.Parse(_configuration["Jwt:ExpireDays"] ?? "7");
        }

        public string GenerateTokenForCliente(Cliente cliente)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email ?? string.Empty),
                new Claim("UserType", "Cliente"),
                new Claim(ClaimTypes.Role, "Cliente")
            };

            return GenerateToken(claims);
        }

        public string GenerateTokenForProfissional(Profissional profissional)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, profissional.Id.ToString()),
                new Claim(ClaimTypes.Name, profissional.Nome),
                new Claim(ClaimTypes.Email, profissional.Email ?? string.Empty),
                new Claim("UserType", "Profissional"),
                new Claim(ClaimTypes.Role, "Profissional")
            };

            return GenerateToken(claims);
        }

        public string GenerateTokenForAdmin(Admin admin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.Nome),
                new Claim(ClaimTypes.Email, admin.Email ?? string.Empty),
                new Claim("UserType", "Admin"),
                new Claim("GithubUsername", admin.GithubUsername),
                new Claim(ClaimTypes.Role, "Admin")
            };

            return GenerateToken(claims);
        }

        public bool ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public string? GetUserTypeFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            
            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userTypeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserType");
                
                return userTypeClaim?.Value;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_expireDays);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
