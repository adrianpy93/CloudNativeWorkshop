#region

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dometrain.Monolith.Api.Identity.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace Dometrain.Monolith.Api.Identity.Services;

public class IdentityService(IOptions<IdentitySettings> identitySettings) : IIdentityService
{
    public string GenerateToken(Guid userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(identitySettings.Value.Key);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, email),
            new(JwtRegisteredClaimNames.Email, email),
            new("user_id", userId.ToString())
        };

        if (email.EndsWith("@dometrain.com"))
        {
            // User is Dometrain employee
            var claim = new Claim("is_admin", "true", ClaimValueTypes.Boolean);
            claims.Add(claim);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(identitySettings.Value.Lifetime),
            Issuer = identitySettings.Value.Issuer,
            Audience = identitySettings.Value.Audience,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}