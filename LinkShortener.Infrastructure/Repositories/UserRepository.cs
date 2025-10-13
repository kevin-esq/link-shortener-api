using LinkShortener.Application.Abstractions;
using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="User"/> entities in the database.
    /// </summary>
    /// <remarks>
    /// Provides data access operations for user management such as registration,
    /// retrieval by ID or email, and existence verification. This repository interacts
    /// directly with <see cref="ApplicationDbContext"/> using Entity Framework Core.
    /// </remarks>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">The database context used for persistence operations.</param>
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier (GUID) of the user.</param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The <see cref="User"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Includes the user's associated <see cref="Domain.Entities.Link"/> collection.
        /// Uses <see cref="EntityFrameworkQueryableExtensions.AsNoTracking"/> to improve
        /// performance for read-only queries.
        /// </remarks>
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Links)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The user's email address (case-insensitive).</param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// The <see cref="User"/> entity if found; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// Email comparisons are performed in a case-insensitive manner by converting
        /// the input to lowercase invariant form before querying.
        /// </remarks>
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .Include(u => u.Links)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        }

        /// <summary>
        /// Adds a new user to the database context.
        /// </summary>
        /// <param name="user">The user entity to be added.</param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        /// <remarks>
        /// The entity will be tracked by the <see cref="ApplicationDbContext"/> until
        /// <see cref="SaveChangesAsync"/> is called to persist changes.
        /// </remarks>
        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        /// <summary>
        /// Persists all pending changes to the database.
        /// </summary>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Determines whether a user with the specified email already exists in the system.
        /// </summary>
        /// <param name="email">The email address to check (case-insensitive).</param>
        /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// <c>true</c> if a user with the given email exists; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is commonly used during registration to ensure email uniqueness.
        /// </remarks>
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
        }
    }
}
