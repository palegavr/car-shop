using CarShop.CarStorage.Database.Entities;
using CarShop.CarStorage.Database.Entities.AdditionalCarOption;
using CarShop.CarStorage.Database.Entities.Car;
using CarShop.CarStorage.Database.Entities.CarEditProcess;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Database
{
    public class CarStorageDatabase : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<AdditionalCarOption> AdditionalCarOptions { get; set; }
        public DbSet<CarConfiguration> CarConfigurations { get; set; }
        public DbSet<CarEditProcess> CarEditProcesses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CarEditProcess>()
                .OwnsOne(e => e.Process);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set.");
            }
            
            connectionString = connectionString.Replace("{database_name}", "carstoragedb");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}