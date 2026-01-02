using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Model;

namespace MonitoringSystem.Controller
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateOrder()
        {
            var order = new Order();
            _logger.LogInformation("Order {OrderId} created", order.Id);

            // TODO:
            // 1. Persist order
            // 2. Enqueue background job
            // 3. Call payment service

            return Accepted(new { order.Id });
        }
    }
}
