using MonitoringSystem.Model;
using OrderWorker.Api;
using OrderWorker.Utility;
using System.Diagnostics;

namespace OrderWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOrderApi _orderApi;

        public Worker(ILogger<Worker> logger, IOrderApi orderApi)
        {
            _logger = logger;
            _orderApi = orderApi;
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
                TracingWorkerTelemetry.ActivitySource
                    .StartActivity("OrderWorker.FetchPendingOrders", ActivityKind.Internal);

            var ids = await _orderApi.GetPendingOrdersAsync();

            foreach (var id in ids)
            {
                await _orderApi.UpdateOrderStatus(id, token);
            }
        }
    }
}
