using MonitoringSystem.Model;
using MonitoringSystem.Repository;

namespace MonitoringSystem.Service
{
    //nothing much doing here yet
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IReadOnlyList<Order>> GetPendingAsync()
        {
            return await _orderRepository.GetPendingAsync();
        }

        public async Task UpdateStatusOrder(Order order)
        {
            await _orderRepository.UpdateStatusOrder(order);
        }
    }
}
