using MonitoringSystem.Model;

namespace MonitoringSystem.Service
{
    public interface IOrderService
    {
        Task CreateAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid id);
        Task UpdateStatusOrder(Guid id);
        Task<IReadOnlyList<Guid>> GetPendingOrderIdsAsync();
    }
}
