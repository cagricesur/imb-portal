using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IMBP.App.Domain;
using IMBP.App.Domain.Contracts;
using IMBP.App.Domain.Settings;
using IMBP.App.Domain.Specifications;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace IMBP.App.Core.Services
{
    internal class TokenService(IOptions<JwtSettings> jwtOptions) : ITokenService
    {
        private readonly JwtSettings jwtSettings = jwtOptions.Value;

        public string CreateToken(TokenUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var role = Enum.GetName(typeof(UserRoleEnum), user.Role) ?? UserRoleEnum.Member.ToString();

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UID.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new(ClaimTypes.NameIdentifier, user.UID.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Role, role),
            };

            if (!string.IsNullOrWhiteSpace(user.MiddleName))
            {
                claims.Add(new Claim("middle_name", user.MiddleName));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
