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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
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
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(u => u.IsActive)
                       .IsRequired();

                builder.Property(u => u.IsEmailVerified)
                       .IsRequired()
                       .HasDefaultValue(false);

                builder.Property(u => u.EmailVerifiedAt)
                       .IsRequired(false)
                       .HasColumnType("datetime2");

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

                builder.Property(u => u.SuspendedAt)
                       .HasColumnType("datetime2");

                builder.Property(u => u.SuspensionReason)
                       .HasMaxLength(500);

                builder.Property(u => u.LastLoginAt)
                       .HasColumnType("datetime2");

                builder.HasIndex(u => new { u.Email, u.AuthProvider })
                       .IsUnique();

                var rolesValueComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<UserRole>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList());

                var rolesProperty = builder.Property<List<UserRole>>("_roles")
                       .HasColumnName("Roles")
                       .HasConversion(
                           v => string.Join(',', v.Select(r => r.ToString())),
                           v => v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(r => Enum.Parse<UserRole>(r))
                                 .ToList())
                       .Metadata;
                
                rolesProperty.SetValueComparer(rolesValueComparer);
                rolesProperty.SetField("_roles");

                builder.HasMany<Link>("_links")
                       .WithOne(l => l.User)
                       .HasForeignKey(l => l.UserId)
                       .OnDelete(DeleteBehavior.Restrict);

                builder.Navigation("_links")
                       .UsePropertyAccessMode(PropertyAccessMode.Field);

                builder.Ignore(u => u.Links);
                builder.Ignore(u => u.Roles);
            });
        }

        private static void ConfigureLinkEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>(builder =>
            {
                builder.HasKey(l => l.Id);

                builder.Property(l => l.LongUrl)
                       .IsRequired()
                       .HasColumnType("nvarchar(max)");

                builder.Property(l => l.Code)
                       .IsRequired()
                       .HasMaxLength(NumberOfCharsInShortlink);

                builder.Property(l => l.CreatedOnUtc)
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(l => l.UpdatedOnUtc)
                       .IsRequired(false)
                       .HasColumnType("datetime2");

                builder.HasIndex(l => l.Code)
                       .IsUnique();

                builder.HasMany<LinkAccess>("_accesses")
                       .WithOne(a => a.Link)
                       .HasForeignKey(a => a.LinkId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.Navigation("_accesses")
                       .UsePropertyAccessMode(PropertyAccessMode.Field);

                builder.Ignore(l => l.Accesses);
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
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Ignore(a => a.Link);

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
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(rt => rt.ExpiresAt)
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(rt => rt.IsRevoked)
                       .IsRequired();

                builder.Property(rt => rt.IsUsed)
                       .IsRequired();

                builder.Property(rt => rt.RevokedAt)
                       .HasColumnType("datetime2");

                builder.Property(rt => rt.UsedAt)
                       .HasColumnType("datetime2");

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
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(s => s.LastActivityAt)
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.Property(s => s.EndedAt)
                       .HasColumnType("datetime2");

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
