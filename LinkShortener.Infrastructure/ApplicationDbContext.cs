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
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<ClickEvent> ClickEvents { get; set; } = null!;
        public DbSet<LinkMetric> LinkMetrics { get; set; } = null!;
        public DbSet<UserMetric> UserMetrics { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
            ConfigureUserRoleEntity(modelBuilder);
            ConfigureLinkEntity(modelBuilder);
            ConfigureLinkAccessEntity(modelBuilder);
            ConfigureRefreshTokenEntity(modelBuilder);
            ConfigureSessionEntity(modelBuilder);
            ConfigureAuditLogEntity(modelBuilder);
            ConfigureClickEventEntity(modelBuilder);
            ConfigureLinkMetricEntity(modelBuilder);
            ConfigureUserMetricEntity(modelBuilder);
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

                // Basic tracking
                builder.Property(a => a.IpAddress)
                       .IsRequired()
                       .HasMaxLength(45);

                builder.Property(a => a.UserAgent)
                       .IsRequired()
                       .HasMaxLength(512);

                builder.Property(a => a.Referer)
                       .HasMaxLength(2048);

                builder.Property(a => a.AccessedOnUtc)
                       .IsRequired();

                // Geographic data
                builder.Property(a => a.Country)
                       .HasMaxLength(100);

                builder.Property(a => a.City)
                       .HasMaxLength(100);

                // Browser & Device analytics
                builder.Property(a => a.Browser)
                       .HasMaxLength(50);

                builder.Property(a => a.BrowserVersion)
                       .HasMaxLength(20);

                builder.Property(a => a.OperatingSystem)
                       .HasMaxLength(50);

                builder.Property(a => a.DeviceType)
                       .HasMaxLength(20);

                builder.Property(a => a.DeviceBrand)
                       .HasMaxLength(50);

                // Relationships
                builder.HasOne(a => a.User)
                       .WithMany()
                       .HasForeignKey(a => a.UserId)
                       .OnDelete(DeleteBehavior.SetNull);

                // Indexes for analytics queries
                builder.HasIndex(a => a.AccessedOnUtc);
                builder.HasIndex(a => a.Country);
                builder.HasIndex(a => a.DeviceType);
                builder.HasIndex(a => a.Browser);
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

        private static void ConfigureAuditLogEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>(builder =>
            {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Timestamp).IsRequired();
                builder.Property(a => a.ActorUsername).IsRequired().HasMaxLength(255);
                builder.Property(a => a.ActorRole).IsRequired().HasMaxLength(50);
                builder.Property(a => a.IpAddress).IsRequired().HasMaxLength(45);
                builder.Property(a => a.UserAgent).IsRequired().HasMaxLength(512);
                builder.Property(a => a.Action).IsRequired().HasMaxLength(100);
                builder.Property(a => a.TargetType).IsRequired().HasMaxLength(50);
                builder.Property(a => a.TargetDisplay).HasMaxLength(500);
                builder.Property(a => a.Outcome).IsRequired().HasMaxLength(50);
                builder.Property(a => a.RequestId).HasMaxLength(100);
                builder.Property(a => a.TraceId).HasMaxLength(100);

                builder.HasIndex(a => a.Timestamp);
                builder.HasIndex(a => a.ActorId);
                builder.HasIndex(a => a.Action);
                builder.HasIndex(a => a.TargetType);
                builder.HasIndex(a => new { a.ActorId, a.Timestamp });
            });
        }

        private static void ConfigureClickEventEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClickEvent>(builder =>
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Timestamp).IsRequired();
                builder.Property(c => c.LinkId).IsRequired();
                builder.Property(c => c.ShortCode).IsRequired().HasMaxLength(20);
                builder.Property(c => c.Destination).IsRequired().HasMaxLength(2048);
                builder.Property(c => c.Referrer).HasMaxLength(2048);
                builder.Property(c => c.UtmSource).HasMaxLength(100);
                builder.Property(c => c.UtmMedium).HasMaxLength(100);
                builder.Property(c => c.UtmCampaign).HasMaxLength(100);
                builder.Property(c => c.UtmContent).HasMaxLength(100);
                builder.Property(c => c.UtmTerm).HasMaxLength(100);
                builder.Property(c => c.IpAddress).IsRequired().HasMaxLength(45);
                builder.Property(c => c.Country).HasMaxLength(100);
                builder.Property(c => c.Region).HasMaxLength(100);
                builder.Property(c => c.City).HasMaxLength(100);
                builder.Property(c => c.DeviceType).IsRequired().HasMaxLength(20);
                builder.Property(c => c.Os).HasMaxLength(50);
                builder.Property(c => c.OsVersion).HasMaxLength(20);
                builder.Property(c => c.Browser).HasMaxLength(50);
                builder.Property(c => c.BrowserVersion).HasMaxLength(20);
                builder.Property(c => c.UserAgent).IsRequired().HasMaxLength(512);
                builder.Property(c => c.AcceptLanguage).HasMaxLength(50);
                builder.Property(c => c.Status).IsRequired().HasMaxLength(50);
                builder.Property(c => c.RedirectType).HasMaxLength(10);
                builder.Property(c => c.RequestId).HasMaxLength(100);
                builder.Property(c => c.TraceId).HasMaxLength(100);

                builder.HasIndex(c => c.Timestamp);
                builder.HasIndex(c => c.LinkId);
                builder.HasIndex(c => c.UserId);
                builder.HasIndex(c => c.Country);
                builder.HasIndex(c => c.DeviceType);
                builder.HasIndex(c => c.Status);
                builder.HasIndex(c => new { c.LinkId, c.Timestamp });
                builder.HasIndex(c => new { c.UserId, c.Timestamp });
            });
        }

        private static void ConfigureLinkMetricEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LinkMetric>(builder =>
            {
                builder.HasKey(m => m.Id);
                builder.Property(m => m.LinkId).IsRequired();
                builder.Property(m => m.Date).IsRequired();
                builder.Property(m => m.TopCountry).HasMaxLength(100);
                builder.Property(m => m.TopDevice).HasMaxLength(20);
                builder.Property(m => m.TopBrowser).HasMaxLength(50);
                builder.Property(m => m.TopReferrer).HasMaxLength(500);

                builder.HasIndex(m => m.LinkId);
                builder.HasIndex(m => m.Date);
                builder.HasIndex(m => new { m.LinkId, m.Date }).IsUnique();
            });
        }

        private static void ConfigureUserMetricEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserMetric>(builder =>
            {
                builder.HasKey(m => m.Id);
                builder.Property(m => m.UserId).IsRequired();
                builder.Property(m => m.Date).IsRequired();

                builder.HasIndex(m => m.UserId);
                builder.HasIndex(m => m.Date);
                builder.HasIndex(m => new { m.UserId, m.Date }).IsUnique();
            });
        }
    }
}
