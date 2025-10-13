using LinkShortener.Domain.Common;
using LinkShortener.Domain.Events;

namespace LinkShortener.Domain.Entities
{
    public class User : BaseEntity
    {
        private User() { }

        public static User Create(string username, string email, string passwordHash, IEnumerable<UserRole>? roles = null)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = ValidateUsername(username),
                Email = ValidateEmail(email),
                PasswordHash = ValidatePasswordHash(passwordHash),
                CreatedOnUtc = DateTime.UtcNow,
                IsActive = true
            };

            if (roles != null)
            {
                foreach (var role in roles)
                    user._roles.Add(role);
            }
            else
            {
                user._roles.Add(UserRole.User);
            }

            return user;
        }

        private readonly List<Link> _links = new List<Link>();
        private readonly List<UserRole> _roles = new List<UserRole>();

        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime CreatedOnUtc { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Link> Links => _links.AsReadOnly();
        public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

        #region Behavior
        public void Deactivate() => IsActive = false;

        public void UpdateEmail(string newEmail)
        {
            Email = ValidateEmail(newEmail);
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = ValidatePasswordHash(newPasswordHash);
        }

        public Link AddLink(string longUrl, string code)
        {
            var link = Link.Create(longUrl, code, Id);
            _links.Add(link);

            AddDomainEvent(new ShortenedUrlCreatedEvent(link.Id, Id, code));

            return link;
        }

        public void AddRole(UserRole role)
        {
            if (!_roles.Contains(role))
                _roles.Add(role);
        }

        public void RemoveRole(UserRole role)
        {
            if (_roles.Contains(role))
                _roles.Remove(role);
        }
        #endregion

        #region Validation
        private static string ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", nameof(username));

            if (username.Length < 3 || username.Length > 30)
                throw new ArgumentException("Username must be between 3 and 30 characters.", nameof(username));

            return username.Trim();
        }

        private static string ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format.", nameof(email));

            return email.Trim().ToLowerInvariant();
        }

        private static string ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

            return passwordHash;
        }
        #endregion
    }

    public enum UserRole
    {
        User,
        Admin
    }
}
