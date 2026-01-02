using Dapper;
using Microsoft.Data.Sqlite;
using MonitoringSystem.Controller;
using MonitoringSystem.Model;
using MonitoringSystem.Utility;

namespace MonitoringSystem.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OrderRepository> _logger;


        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("OrdersDb")!;
            _logger = logger;
        }

        public async Task CreateAsync(Order order)
        {
            const string sqlQuery = """
            INSERT INTO Orders (Id, CreatedAt, Status)
            VALUES (@Id, @CreatedAt, @Status);
        """;

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                await connection.ExecuteAsync(sqlQuery, order);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create order with Id {OrderId}", order.Id);
                throw new OrderPersistenceException("Database failure", ex);
            }
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            const string sqlQuery = """
            SELECT Id, CreatedAt, Status
            FROM Orders
            WHERE Id = @Id;
        """;

            try
            {
                using var connection = new SqliteConnection(_connectionString);
                return await connection.QuerySingleOrDefaultAsync<Order>(
                    sqlQuery, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve order with Id {OrderId}", id);
                throw new OrderPersistenceException("Database failure", ex);
            }
        }
    }
}
