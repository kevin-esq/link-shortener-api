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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUserEntity(modelBuilder);
            ConfigureLinkEntity(modelBuilder);
            ConfigureLinkAccessEntity(modelBuilder);
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
                       .IsRequired()
                       .HasColumnType("datetime2");

                builder.HasOne(a => a.Link)
                       .WithMany(l => l.Accesses)
                       .HasForeignKey(a => a.LinkId)
                       .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(a => a.User)
                       .WithMany()
                       .HasForeignKey(a => a.UserId)
                       .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
