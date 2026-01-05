using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Model;
using MonitoringSystem.Repository;

namespace MonitoringSystem.Controller
{
    [Route("metrics/orders")]
    [ApiController]
    public class OrderMetricsController : ControllerBase
    {
        private readonly IOrderMetricsRepository _orderMetricsRepository;

        public OrderMetricsController(IOrderMetricsRepository orderMetricsRepository)
        {
            _orderMetricsRepository = orderMetricsRepository;
        }

        [HttpGet("summary")]
        public async Task<ActionResult<OrderSummaryDto>> GetSummary()
            => Ok(await _orderMetricsRepository.GetOrderSummaryAsync());

        [HttpGet("retries")]
        public async Task<ActionResult<IReadOnlyList<OrderRetryMetricDto>>> GetRetries()
            => Ok(await _orderMetricsRepository.GetRetryDistributionAsync());

    }
}
