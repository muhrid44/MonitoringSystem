using MonitoringSystem.Repository;
using MonitoringSystem.Service;

namespace MonitoringSystem.Utility
{
    public static class ComponentRegistration
    {
        public static void RegisterComponents(IServiceCollection services)
        {
            // Register services
            services.AddScoped<IOrderService, OrderService>();
            // Register repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
