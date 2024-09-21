using CarShop.ServiceDefaults.CommonTypes;
using Microsoft.EntityFrameworkCore;

namespace CarShop.CarStorage.Database
{
    public class CarStorageDatabase : DbContext
    {
        public DbSet<Car> Cars { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=testdb;Username=postgres;Password=123");
        }
    }
}
