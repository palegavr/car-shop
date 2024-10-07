using CarShop.AdminService.Database;
using CarShop.ServiceDefaults.ServiceInterfaces.AdminService;
using Microsoft.EntityFrameworkCore;

namespace CarShop.AdminService.Repositories
{
    public class RefreshSessionsRepository(AdminServiceDatabase _db)
    {
        public async Task CreateSessionAsync(RefreshSession refreshSession)
        {
            _db.RefreshSessions.Add(refreshSession);
            await _db.SaveChangesAsync();
            _db.Entry(refreshSession).State = EntityState.Detached;
        }
    }
}
