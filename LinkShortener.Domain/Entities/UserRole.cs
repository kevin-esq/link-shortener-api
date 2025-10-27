using LinkShortener.Domain.Common;

namespace LinkShortener.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        private UserRole() { }

        public static UserRole Create(Guid userId, Role role, Guid? grantedBy = null)
        {
            return new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = role,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = grantedBy
            };
        }

        public Guid UserId { get; private set; }
        public Role Role { get; private set; }
        public DateTime GrantedAt { get; private set; }
        public Guid? GrantedBy { get; private set; }

        public User User { get; private set; } = null!;
        public User? GrantedByUser { get; private set; }
    }

    public enum Role
    {
        User = 0,
        Premium = 1,
        Admin = 2
    }
}
