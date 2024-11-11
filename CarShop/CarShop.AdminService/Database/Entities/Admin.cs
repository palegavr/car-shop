using System.ComponentModel.DataAnnotations;
using CarShop.ServiceDefaults;

namespace CarShop.AdminService.Database.Entities
{
    public class Admin
    {
        [Key] public long Id { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string[] Roles { get; set; } = [];

        [Range(Constants.HighestAdminPriority, Constants.LowestAdminPriority)]
        public int Priority { get; set; }

        public bool Banned { get; set; }
    }
}