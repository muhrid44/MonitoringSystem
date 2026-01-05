using Microsoft.AspNetCore.Mvc;
using MonitoringSystem.Model;
using MonitoringSystem.Repository;

namespace MonitoringSystem.Controller
{
    [ApiController]
    [Route("dev/seed")]
    public class DevSeedController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DevSeedController> _logger;

        public DevSeedController(
            IOrderRepository repo,
            IWebHostEnvironment env,
            ILogger<DevSeedController> logger)
        {
            _repo = repo;
            _env = env;
            _logger = logger;
        }

        [HttpPost("orders")]
        public async Task<IActionResult> SeedOrders([FromQuery] int count = 1000)
        {
            if (!_env.IsDevelopment())
                return NotFound(); // pretend it doesn't exist

            _logger.LogWarning("DEV SEED invoked. Clearing orders table.");

            await _repo.DeleteAllAsync();

            for (int i = 0; i < count; i++)
            {
                var newOrder = new Order();
                newOrder.Id = Guid.NewGuid();
                await _repo.CreateAsync(newOrder);
            }

            return Ok(new { Seeded = count });
        }
    }

}
