using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarShop.AdminService.Database.Entities
{
    public class RefreshSession
    {
        [Key]
        public long Id { get; set; }
        public long AdminId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped] public bool IsExpired => DateTime.UtcNow > ExpiresIn;
    }
}
