﻿using CarShop.AdminService.Database;
using CarShop.AdminService.Database.Entities;
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

        public async Task RemoveAllSessionsOfAdminAsync(long id)
        {
            RefreshSession[] refreshSessions = await _db.RefreshSessions
                .Where(session => session.AdminId == id).ToArrayAsync();

            if (refreshSessions.Length > 0)
            {
                _db.RefreshSessions.RemoveRange(refreshSessions);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteSessionAsync(long id)
        {
            _db.RefreshSessions.Remove(new RefreshSession { Id = id });
            await _db.SaveChangesAsync();
        }
    }
}
