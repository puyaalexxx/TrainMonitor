using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;

namespace TrainMonitor.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();

            await using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                await context.Database.MigrateAsync();

                app.Logger.LogInformation("Database migration applied successfully.");
            }
            catch (Exception e)
            {
                app.Logger.LogError(e, "An error occurred while applying database migration.");

                throw;
            }

        }
    }
}
