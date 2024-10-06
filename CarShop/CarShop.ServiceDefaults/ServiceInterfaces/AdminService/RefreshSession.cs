using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.ServiceDefaults.ServiceInterfaces.AdminService
{
    public class RefreshSession
    {
        [Key]
        public long Id { get; set; }
        public long AdminId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
