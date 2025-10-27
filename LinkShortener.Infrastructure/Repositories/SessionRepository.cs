using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Session>()
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<Session>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<Session>()
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.LastActivityAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<Session>()
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Session session, CancellationToken cancellationToken)
        {
            await _context.Set<Session>().AddAsync(session, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task EndAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var sessions = await _context.Set<Session>()
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var session in sessions)
            {
                session.End();
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.Set<Session>()
                .CountAsync(s => s.UserId == userId && s.IsActive, cancellationToken);
        }
    }
}
