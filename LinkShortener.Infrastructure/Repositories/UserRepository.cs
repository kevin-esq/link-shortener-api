using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Links)
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Links)
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        }

        public async Task<List<User>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Links)
                .Include(u => u.UserRoles)
                .OrderByDescending(u => u.CreatedOnUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        }
    }
}
