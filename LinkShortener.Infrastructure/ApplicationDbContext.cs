using LinkShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        private const int NumberOfCharsInShortlink = 7;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Link> Links { get; set; } = null!;
        public DbSet<LinkAccess> LinkAccesses { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
            ConfigureUserRoleEntity(modelBuilder);
            ConfigureLinkEntity(modelBuilder);
            ConfigureLinkAccessEntity(modelBuilder);
            ConfigureRefreshTokenEntity(modelBuilder);
            ConfigureSessionEntity(modelBuilder);
        }

        private static void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.HasKey(u => u.Id);

                builder.Property(u => u.Username)
                       .IsRequired()
                       .HasMaxLength(30);

                builder.Property(u => u.Email)
                       .IsRequired()
                       .HasMaxLength(255);

                builder.Property(u => u.PasswordHash)
                       .IsRequired();

                builder.Property(u => u.CreatedOnUtc)
                       .IsRequired();

                builder.Property(u => u.IsActive)
                       .IsRequired();

                builder.Property(u => u.IsEmailVerified)
                       .IsRequired();

                builder.Property(u => u.EmailVerifiedAt)
                       .IsRequired(false);

                builder.Property(u => u.AuthProvider)
                       .IsRequired()
                       .HasConversion<string>()
                       .HasMaxLength(20);

                builder.Property(u => u.ExternalProviderId)
                       .IsRequired(false)
                       .HasMaxLength(255);

                builder.Property(u => u.Status)
                       .IsRequired()
                       .HasConversion<string>()
                       .HasMaxLength(30);

                builder.Property(u => u.SuspendedAt);

                builder.Property(u => u.SuspensionReason)
                       .HasMaxLength(500);

                builder.Property(u => u.LastLoginAt);

                builder.HasIndex(u => new { u.Email, u.AuthProvider })
                       .IsUnique();
            });
        }

        private static void ConfigureUserRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>(builder =>
            {
                builder.HasKey(ur => ur.Id);

                builder.Property(ur => ur.UserId)
                       .IsRequired();

                builder.Property(ur => ur.Role)
                       .IsRequired()
                       .HasConversion<string>()
                       .HasMaxLength(20);

                builder.Property(ur => ur.GrantedAt)
                       .IsRequired();

                builder.Property(ur => ur.GrantedBy)
                       .IsRequired(false);

                builder.HasIndex(ur => new { ur.UserId, ur.Role })
                       .IsUnique();

                builder.HasOne(ur => ur.User)
                       .WithMany(u => u.UserRoles)
                       .HasForeignKey(ur => ur.UserId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(ur => ur.GrantedByUser)
                       .WithMany()
                       .HasForeignKey(ur => ur.GrantedBy)
                       .OnDelete(DeleteBehavior.NoAction);
            });
        }

        private static void ConfigureLinkEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>(builder =>
            {
                builder.HasKey(l => l.Id);

                builder.Property(l => l.LongUrl)
                       .IsRequired();

                builder.Property(l => l.Code)
                       .IsRequired()
                       .HasMaxLength(NumberOfCharsInShortlink);

                builder.Property(l => l.CreatedOnUtc)
                       .IsRequired();

                builder.Property(l => l.UpdatedOnUtc)
                       .IsRequired(false);

                builder.HasIndex(l => l.Code)
                       .IsUnique();

                builder.HasOne(l => l.User)
                       .WithMany(u => u.Links)
                       .HasForeignKey(l => l.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(l => l.Accesses)
                       .WithOne(a => a.Link)
                       .HasForeignKey(a => a.LinkId)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureLinkAccessEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkAccess>(builder =>
            {
                builder.HasKey(a => a.Id);

                builder.Property(a => a.IpAddress)
                       .IsRequired()
                       .HasMaxLength(45);

                builder.Property(a => a.UserAgent)
                       .IsRequired()
                       .HasMaxLength(512);

                builder.Property(a => a.AccessedOnUtc)
                       .IsRequired();

                builder.HasOne(a => a.User)
                       .WithMany()
                       .HasForeignKey(a => a.UserId)
                       .OnDelete(DeleteBehavior.SetNull);
            });
        }

        private static void ConfigureRefreshTokenEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshToken>(builder =>
            {
                builder.HasKey(rt => rt.Id);

                builder.Property(rt => rt.Token)
                       .IsRequired()
                       .HasMaxLength(500);

                builder.Property(rt => rt.CreatedAt)
                       .IsRequired();

                builder.Property(rt => rt.ExpiresAt)
                       .IsRequired();

                builder.Property(rt => rt.IsRevoked)
                       .IsRequired();

                builder.Property(rt => rt.IsUsed)
                       .IsRequired();

                builder.Property(rt => rt.RevokedAt);

                builder.Property(rt => rt.UsedAt);

                builder.Property(rt => rt.ReplacedByToken)
                       .HasMaxLength(500);

                builder.HasIndex(rt => rt.Token)
                       .IsUnique();

                builder.HasIndex(rt => rt.UserId);

                builder.HasOne(rt => rt.User)
                       .WithMany()
                       .HasForeignKey(rt => rt.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private static void ConfigureSessionEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(builder =>
            {
                builder.HasKey(s => s.Id);

                builder.Property(s => s.IpAddress)
                       .IsRequired()
                       .HasMaxLength(45);

                builder.Property(s => s.UserAgent)
                       .IsRequired()
                       .HasMaxLength(500);

                builder.Property(s => s.DeviceName)
                       .HasMaxLength(200);

                builder.Property(s => s.Location)
                       .HasMaxLength(200);

                builder.Property(s => s.CreatedAt)
                       .IsRequired();

                builder.Property(s => s.LastActivityAt)
                       .IsRequired();

                builder.Property(s => s.EndedAt);

                builder.Property(s => s.IsActive)
                       .IsRequired();

                builder.Property(s => s.RefreshTokenId)
                       .HasMaxLength(100);

                builder.HasIndex(s => s.UserId);
                builder.HasIndex(s => new { s.UserId, s.IsActive });

                builder.HasOne(s => s.User)
                       .WithMany()
                       .HasForeignKey(s => s.UserId)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
