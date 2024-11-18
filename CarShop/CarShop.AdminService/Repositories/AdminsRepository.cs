using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using CarShop.AdminService.Database;
using CarShop.AdminService.Database.Entities;
using CarShop.AdminService.Grpc;
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

        public async Task<Admin[]> GetMany(
            GetAccountsRequest.Types.SortType sortType = GetAccountsRequest.Types.SortType.Asc,
            GetAccountsRequest.Types.SortBy? sortBy = null,
            int? minPriority = null,
            int? maxPriority = null,
            string[]? haveRoles = null,
            bool? banned = null)
        {
            var query = _db.Admins.AsNoTracking();

            if (minPriority is not null)
            {
                query = query.Where(admin => admin.Priority >= minPriority);
            }
            
            if (maxPriority is not null)
            {
                query = query.Where(admin => admin.Priority <= maxPriority);
            }

            if (haveRoles is not null)
            {
                query = query.Where(admin => admin.Roles.Any(haveRoles.Contains));
            }

            if (banned is not null)
            {
                query = query.Where(admin => admin.Banned == banned);
            }
            
            query = sortBy switch
            {
                GetAccountsRequest.Types.SortBy.Priority => sortType == GetAccountsRequest.Types.SortType.Asc
                    ? query.OrderBy(admin => admin.Priority)
                    : query.OrderByDescending(admin => admin.Priority),
                GetAccountsRequest.Types.SortBy.Banned => sortType == GetAccountsRequest.Types.SortType.Asc
                    ? query.OrderBy(admin => admin.Banned)
                    : query.OrderByDescending(admin => admin.Banned),
                _ => query
            };
            
            return (await query.ToArrayAsync())
                .Select(admin =>
                {
                    admin.Roles = PrepareRoles(admin.Roles);
                    return admin;
                })
                .ToArray();
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