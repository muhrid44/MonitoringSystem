using MonitoringSystem.Model;

namespace MonitoringSystem.Repository
{
    public interface IOrderRepository
    {
        Task CreateAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid id, bool isPending = false);
        Task<IReadOnlyList<Guid>> GetPendingOrderIdsAsync();
        Task UpdateStatusOrder(Order order);
        Task DeleteAllAsync();
    }
}
