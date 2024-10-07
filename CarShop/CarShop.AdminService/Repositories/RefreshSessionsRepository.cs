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

        public async Task<RefreshSession?> GetByRefreshTokenAsync(string refreshToken)
        {
            RefreshSession? refreshSession = await _db.RefreshSessions.SingleOrDefaultAsync(session => session.RefreshToken == refreshToken);
            if (refreshSession is not null)
            {
                _db.Entry(refreshSession).State = EntityState.Detached;
            }
            return refreshSession;
        }

        public async Task UpdateSessionAsync(RefreshSession refreshSession)
        {
            _db.RefreshSessions.Update(refreshSession);
            await _db.SaveChangesAsync();
            _db.Entry(refreshSession).State = EntityState.Detached;
        }
    }
}
