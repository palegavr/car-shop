using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService.Database
{
    public class AdminServiceDatabase : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<RefreshSession> RefreshSessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string host = "db";
            int port = 5432;
            string database = "adminsdb";
            string username = "postgres";
            string password = "123";
            optionsBuilder.UseNpgsql($"Host={host};Port={port};Database={database};Username={username};Password={password}");
        }
    }
}
