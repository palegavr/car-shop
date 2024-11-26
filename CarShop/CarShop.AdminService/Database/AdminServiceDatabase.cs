using CarShop.AdminService.Database.Entities;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService.Database
{
    public class AdminServiceDatabase : DbContext
    {
        public DbSet<Admin> Admins { get; set; }
        public DbSet<RefreshSession> RefreshSessions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(new Admin
            {
                Id = 1,
                Email = "admin@admin.com",
                Password =
                    "$argon2id$v=19$m=65536,t=3,p=1$iICM/5uHlAHETRq8PtSHxg$jnk1HHpTP4voBpY80g5LCciaToO9WNT4X4IM7FL2KKk",
                Banned = false,
                Priority = 1,
                Roles = [Constants.AllRolesSymbol]
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }
            
            connectionString = connectionString.Replace("{database_name}", "adminsdb");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
