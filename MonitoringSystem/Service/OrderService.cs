using MonitoringSystem.Model;
using MonitoringSystem.Repository;

namespace MonitoringSystem.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        //nothing much doing here yet
        public async Task CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        //nothing much doing here yet
        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }
    }
}
