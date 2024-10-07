using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    public class AuthOptions
    {
        public const string ISSUER = "AdminService";
        public const string AUDIENCE = "All";
        private const string KEY = "mysupersecret_secretsecretsecretkey!123";
        public static readonly TimeSpan REFRESH_TOKEN_LIFETIME = new TimeSpan(30, 0, 0, 0); // 30 дней
        public static readonly TimeSpan ACCESS_TOKEN_LIFETIME = new TimeSpan(0, 3, 0); // 3 минуты
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
