using MonitoringSystem.Model;
using MonitoringSystem.Repository;

namespace MonitoringSystem.Utility
{
    public static class DevelopmentSeedingDataDummy
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            ILogger logger,
            int count = 1000)
        {
            using var scope = services.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            logger.LogWarning("DEVELOPMENT MODE: Clearing orders table");
            await repo.DeleteAllAsync();

            logger.LogInformation("Seeding {Count} orders", count);

            for (int i = 0; i < count; i++)
            {
                await repo.CreateAsync(new Order());
            }

            logger.LogInformation("Seeding completed");
        }
    }
}
