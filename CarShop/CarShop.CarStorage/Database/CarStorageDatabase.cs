using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Database
{
    public class CarStorageDatabase : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string host = "db";
            int port = 5432;
            string database = "testdb";
            string username = "postgres";
            string password = "123";
			optionsBuilder.UseNpgsql($"Host={host};Port={port};Database={database};Username={username};Password={password}");
        }
    }
}
