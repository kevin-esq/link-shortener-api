using link_shortener.Entities;
using Microsoft.EntityFrameworkCore;

namespace link_shortener
{
    public class ApplicationDbContext : DbContext
    {
        private const int NumberOfCharsInShortlink = 7;

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureShortenedUrlEntity(modelBuilder);
        }

        private void ConfigureShortenedUrlEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(NumberOfCharsInShortlink);
                builder.HasIndex(s => s.Code).IsUnique();
            });
        }
    }
}
