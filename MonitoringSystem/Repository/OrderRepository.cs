using Dapper;
using Microsoft.Data.Sqlite;
using MonitoringSystem.Controller;
using MonitoringSystem.Model;
using MonitoringSystem.Utility;
using Npgsql;

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
        INSERT INTO orders (id, created_at, status)
           VALUES (@Id, @CreatedAt, @Status);
        """;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.ExecuteAsync(sqlQuery, new
                {
                    Id = order.Id,
                    CreatedAt = DateTime.Now,
                    Status = order.Status
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create order with Id {OrderId}", order.Id);
                throw new OrderPersistenceException("Database failure", ex);
            }
        }

        public async Task DeleteAllAsync()
        {
            const string sqlQuery = """
        DELETE FROM orders
        """;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.ExecuteAsync(sqlQuery);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete order");
                throw new OrderPersistenceException("Database failure", ex);
            }
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id, bool isPending = false)
        {
            const string sqlQuery = """
        SELECT
              id AS Id,
              created_at AS CreatedAt,
              status AS Status
          FROM orders
          WHERE id = @Id;
        """;

            const string sqlQueryPending = """
        SELECT
            id AS Id,
            created_at AS CreatedAt,
            status AS Status,
            retry_count AS RetryCount
        FROM orders
        WHERE id = @Id AND status = @Created
          AND retry_count < @MaxRetry;
        
        """;

            string queryUsed = (bool)isPending ? sqlQueryPending : sqlQuery;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                return await connection.QuerySingleOrDefaultAsync<Order>(
                    queryUsed, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve order with Id {OrderId}", id);
                throw new OrderPersistenceException("Database failure", ex);
            }
        }

        public async Task<IReadOnlyList<Guid>> GetPendingOrderIdsAsync()
        {
            const string sqlQuery = """
        SELECT
            id AS Id
        FROM orders
        WHERE status = @Created
          AND retry_count < @MaxRetry;
        
        """;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                return await connection.QueryFirstAsync<IReadOnlyList<Guid>>(sqlQuery);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve orders");
                throw new OrderPersistenceException("Database failure", ex);
            }
        }

        public async Task UpdateStatusOrder(Order order)
        {
            const string sql = """
                    UPDATE orders
                          SET
                              status = @Status,
                              retry_count = @RetryCount
                          WHERE id = @Id;
                    """;

            try
            {
                using var connection = new NpgsqlConnection(_connectionString);

                var affected = await connection.ExecuteAsync(sql, new
                {
                    Id = order.Id,
                    Status = (int)order.Status,
                    RetryCount = order.RetryCount
                });

                if (affected == 0)
                {
                    _logger.LogWarning(
                        "Order {OrderId} not updated (not found)",
                        order.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to update order {OrderId}",
                    order.Id);

                throw new OrderPersistenceException(
                    "Failed to update order",
                    ex);
            }
        }
    }
}
