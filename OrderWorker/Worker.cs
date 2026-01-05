using MonitoringSystem.Controller;
using MonitoringSystem.Model;
using MonitoringSystem.Service;
using MonitoringSystem.Utility;
using System.Diagnostics;

namespace OrderWorker
{
    public class Worker : BackgroundService
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<Worker> _logger;
        private readonly OrderProcessingOptions _options;

        public Worker(IOrderService orderService, ILogger<Worker> logger, OrderProcessingOptions options)
        {
            _orderService = orderService;
            _logger = logger;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // fetch pending
                // mark processing
                // simulate work
                // mark completed / failed
                await ProcessPendingOrders(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task ProcessPendingOrders(CancellationToken token)
        {
            using var activity =
                TracingSystemTelemetry.ActivitySource
                    .StartActivity("OrderWorker.FetchPendingOrders", ActivityKind.Internal);

            var orders = await _orderService.GetPendingAsync();

            foreach (var order in orders)
            {
                await ProcessOrder(order, token);
            }
        }

        private async Task ProcessOrder(Order order, CancellationToken token)
        {
            using var activity =
                TracingSystemTelemetry.ActivitySource
                    .StartActivity("OrderWorker.ProcessOrder", ActivityKind.Internal);

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

                await _orderService.UpdateStatusOrder(order);

                _logger.LogWarning(
                    "Order {OrderId} failed (retry {Retry}/{MaxRetryCount})",
                    order.Id,
                    order.RetryCount,
                    _options.MaxRetryCount);

                return;
            }


            order.MarkProcessing();
            await _orderService.UpdateStatusOrder(order);

            await Task.Delay(TimeSpan.FromSeconds(2), token);

            order.MarkCompleted();
            await _orderService.UpdateStatusOrder(order);
        }

        private bool ShouldFail()
        {
            return Random.Shared.NextDouble() < _options.FailureRate;
        }


    }
}
