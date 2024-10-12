using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using System.Security.Claims;

    public class TokenValidator
    {
        public static readonly TokenValidationParameters ValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,

            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        };

        public static ClaimsPrincipal? ValidateToken(string token)
        {            
            ClaimsPrincipal claimsPrincipal = 
                new JwtSecurityTokenHandler()
                .ValidateToken(token, ValidationParameters, out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return claimsPrincipal;
            }
            else
            {
                return null;
            }
        }
    }

}
