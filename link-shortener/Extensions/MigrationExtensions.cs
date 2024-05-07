using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using link_shortener;

namespace link_shortener.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            MigrateDatabase(scope.ServiceProvider);
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
        }
    }
}