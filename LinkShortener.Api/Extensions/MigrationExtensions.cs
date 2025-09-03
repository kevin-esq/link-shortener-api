using LinkShortener.Infrastructure;
using Microsoft.EntityFrameworkCore;


namespace LinkShortener.Api.Extensions
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