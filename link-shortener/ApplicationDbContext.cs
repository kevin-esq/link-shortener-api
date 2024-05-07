using link_shortener.Entities;
using link_shortener.Services;
using Microsoft.EntityFrameworkCore;

namespace link_shortener
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        }

        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortenedUrl>(builder =>
            {
                builder.Property(s => s.Code).HasMaxLength(UrlShorteningService.NumberOfCharsInShortlink);

                builder.HasIndex(s => s.Code).IsUnique();
            });
        }
    }
}
