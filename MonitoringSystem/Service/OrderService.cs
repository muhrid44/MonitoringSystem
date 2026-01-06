using MonitoringSystem.Model;
using MonitoringSystem.Repository;
using MonitoringSystem.Utility;
using System.Diagnostics;

namespace MonitoringSystem.Service
{
    //nothing much doing here yet
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderProcessingOptions _options;
        private readonly ILogger<OrderService> _logger;


        public OrderService(IOrderRepository orderRepository, OrderProcessingOptions options, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _options = options;
            _logger = logger;
        }

        public async Task CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IReadOnlyList<Guid>> GetPendingOrderIdsAsync()
        {
            return await _orderRepository.GetPendingOrderIdsAsync();
        }

        public async Task UpdateStatusOrder(Guid id)
        {
            var orderPending = await _orderRepository.GetOrderByIdAsync(id, true);

            if (orderPending == null)
            {
                return;
            }

            await ProcessOrder(orderPending);
        }

        private async Task ProcessOrder(Order order)
        {
            using var activity =
                TracingSystemTelemetry.ActivitySource
                    .StartActivity("MonitoringSystem.ProcessOrder", ActivityKind.Internal);

            activity?.SetTag("order.id", order.Id);
            activity?.SetTag("order.status", order.Status);

            if (ShouldFail())
            {
                order.IncrementRetry();

                if (order.RetryCount >= _options.MaxRetryCount)
                {
                    order.MarkFailed(_options.MaxRetryCount);
                    activity?.SetTag("order.result", "failed_terminal");
                }
                else
                {
                    order.MarkCreated(); // retry
                    activity?.SetTag("order.result", "retry");
                }

                await _orderRepository.UpdateStatusOrder(order);

                _logger.LogWarning(
                    "Order {OrderId} failed (retry {Retry}/{MaxRetryCount})",
                    order.Id,
                    order.RetryCount,
                    _options.MaxRetryCount);

                return;
            }


            order.MarkProcessing();
            await _orderRepository.UpdateStatusOrder(order);

            await Task.Delay(TimeSpan.FromSeconds(2));

            order.MarkCompleted();
            await _orderRepository.UpdateStatusOrder(order);
        }

        private bool ShouldFail()
        {
            return Random.Shared.NextDouble() < _options.FailureRate;
        }
    }
}
