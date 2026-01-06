using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Model;
using MonitoringSystem.Repository;
using MonitoringSystem.Service;
using MonitoringSystem.Utility;
using OpenTelemetry.Metrics;

namespace MonitoringSystem.Controller
{
    [ApiController]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var order = new Order();
            order.Id = Guid.NewGuid();

            try
            {
                await _orderService.CreateAsync(order);
                _logger.LogInformation("Order {OrderId} created", order.Id);
            }
            catch (OrderPersistenceException)
            {
                return StatusCode(500, "Order service is temporarily unavailable");
            }

            return Accepted(new { order.Id });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Order>> GetOrderById(Guid id)
        {
            Order? order;
            try
            {
                order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Order {OrderId} retrieved", id);
            }
            catch (OrderPersistenceException)
            {
                return StatusCode(500, "Order service is temporarily unavailable");
            }

            return Ok(order);
        }

        [HttpGet("get-pending-order-ids")]
        public async Task<IActionResult> GetPendingOrderIds()
        {
            try
            {
                await _orderService.GetPendingOrderIdsAsync();
            }
            catch (OrderPersistenceException)
            {
                return StatusCode(500, "Order service is temporarily unavailable");
            }

            return Ok();
        }

        [HttpPost("{id}/process")]
        public async Task<ActionResult<Order>> UpdateOrderStatus(Guid id)
        {
            try
            {
                await _orderService.UpdateStatusOrder(id);
            }
            catch (OrderPersistenceException)
            {
                return StatusCode(500, "Order service is temporarily unavailable");
            }

            return Ok();
        }

    }
}
