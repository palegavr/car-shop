using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.IdentityModel.Tokens;

namespace CarShop.AdminService.Services;

public class TokensPairGenerator
{
    public TokensPair GenerateTokensPair(long id, string email, string[] roles, int priority,
        out DateTime refreshTokenExpires)
    {
        refreshTokenExpires = DateTime.UtcNow.Add(AuthOptions.REFRESH_TOKEN_LIFETIME);
        JwtSecurityToken refreshJwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            expires: refreshTokenExpires,
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        string refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshJwt);

        var roleClaims = from role in roles select new Claim(ClaimTypes.Role, role);
        var claims = new List<Claim>(roleClaims)
        {
            new("id", id.ToString()),
            new("email", email),
            new("priority", priority.ToString()),
        };

        JwtSecurityToken accessJwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(AuthOptions.ACCESS_TOKEN_LIFETIME),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
        string accessToken = new JwtSecurityTokenHandler().WriteToken(accessJwt);

        return new TokensPair(refreshToken, accessToken);
    }

    public record TokensPair(string RefreshToken, string AccessToken);
}