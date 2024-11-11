using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using CarShop.AdminService.Database;
using CarShop.AdminService.Database.Entities;
using CarShop.ServiceDefaults;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Isopoh.Cryptography.Argon2;
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
                admin.Roles = PrepareRoles(admin.Roles);
                _db.Entry(admin).State = EntityState.Detached;
            }

            return admin;
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            Admin? admin = await _db.Admins.Where(admin => admin.Email == email).SingleOrDefaultAsync();

            if (admin is not null)
            {
                admin.Roles = PrepareRoles(admin.Roles);
                _db.Entry(admin).State = EntityState.Detached;
            }

            return admin;
        }

        public async Task CreateAccountAsync(Admin admin)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(admin.Id, default, nameof(admin.Id));
            admin.Roles = PrepareRoles(admin.Roles);
            Validator.ValidateObject(admin, new(admin), true);
            admin.Password = Argon2.Hash(admin.Password);
            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAccountAsync(long id)
        {
            _db.Admins.Remove(new Admin { Id = id });
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Admin admin)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(admin.Id, nameof(admin.Id));
            Validator.ValidateObject(admin, new(admin), true);
            Admin? otherAdmin = await GetByEmailAsync(admin.Email);
            if (otherAdmin is not null && admin.Id != otherAdmin.Id)
            {
                throw new ArgumentException("Email is busy.", nameof(admin.Email));
            }

            _db.Admins.Update(admin);
            await _db.SaveChangesAsync();
            _db.Entry(admin).State = EntityState.Detached;
        }

        private string[] PrepareRoles(string[] roles)
        {
            if (roles.Contains(Constants.AllRolesSymbol))
            {
                roles = Role.AllExistingRoles;
            }

            return roles;
        }
    }
}