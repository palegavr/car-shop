using CarShop.AdminService.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService.Repositories
{
    public class AdminsRepository(AdminServiceDatabase _db)
    {
        public async Task<Admin?> GetByIdAsync(long id)
        {
            Admin? admin = await _db.Admins.SingleOrDefaultAsync(admin => admin.Id == id);

            if (admin is not null)
            {
                _db.Entry(admin).State = EntityState.Detached;
            }
            
            return admin;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            Admin? admin = await _db.Admins.Where(admin => admin.Email == email).SingleOrDefaultAsync();

            if (admin is not null)
            {
                _db.Entry(admin).State = EntityState.Detached;
            }

            return admin;
        }

        public async Task CreateAccountAsync(string email, string password, int priority, string[]? roles = null)
        {
            _db.Admins.Add(new Admin
            {
                Email = email, 
                Password = password, 
                Roles = roles ?? Constants.DefaultAdminRoles,
                Priority = priority
            });
            await _db.SaveChangesAsync();
        }
        
        public async Task DeleteAccountAsync(long id)
        {
            _db.Admins.Remove(new Admin { Id = id });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Admin admin)
        {
            _db.Admins.Update(admin);
            await _db.SaveChangesAsync();
            _db.Entry(admin).State = EntityState.Detached;
        }
    }
}
