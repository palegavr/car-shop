using CarShop.AdminService.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService.Repositories
{
    public class AdminsRepository(AdminServiceDatabase _db)
    {
        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _db.Admins.Where(admin => admin.Email == email).SingleOrDefaultAsync();
        }

        public async Task CreateAccount(string email, string password)
        {
            _db.Admins.Add(new Admin { Email = email, Password = password });
            await _db.SaveChangesAsync();
        }
    }
}
