using LinkShortener.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShortener.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        private const int NumberOfCharsInShortlink = 7;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureShortenedUrlEntity(modelBuilder);
        }

        private void ConfigureShortenedUrlEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.HasKey(s => s.Id);

                builder.Property(s => s.LongUrl)
                       .IsRequired()
                       .HasColumnType("nvarchar(max)");

                builder.Property(s => s.ShortUrl)
                       .IsRequired()
                       .HasMaxLength(500);

                builder.Property(s => s.Code)
                       .IsRequired()
                       .HasMaxLength(NumberOfCharsInShortlink);

                builder.HasIndex(s => s.Code)
                       .IsUnique();

                builder.Property(s => s.CreateOnUtc)
                       .IsRequired()
                       .HasColumnType("datetime2");
            });
        }
    }
}
