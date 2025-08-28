using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TooliRent.Domain.Entities;
using TooliRent.Infrastructure.Identity;

namespace TooliRent.Application.Authentication
{
    public interface ITokenService
    {
        Task<(string accessToken, DateTime expiresUtc)> CreateAccessTokenAsync(AppUser user);
        string CreateRefreshToken();
    }
    public class TokenService :ITokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _users;

        public TokenService(IConfiguration config, UserManager<AppUser> users)
        {
            _config = config;
            _users = users;
        }

        public async Task<(string accessToken, DateTime expiresUtc)> CreateAccessTokenAsync(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _users.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var minutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 60;
            var expires = DateTime.UtcNow.AddMinutes(minutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expires);
        }

        public string CreateRefreshToken()
        {
            // simple crypto-strong random
            var bytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes);
        }

    }
}
