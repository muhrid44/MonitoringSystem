using Dapper;
using MonitoringSystem.Model;
using Npgsql;

namespace MonitoringSystem.Repository
{
    public class OrderMetricsRepository : IOrderMetricsRepository
    {
        private readonly string _connectionString;

        public OrderMetricsRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("OrdersDb")!;
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync()
        {
            const string sql = """
            SELECT
              COUNT(*) AS Total,
              COUNT(*) FILTER (WHERE status = 0) AS Created,
              COUNT(*) FILTER (WHERE status = 1) AS Processing,
              COUNT(*) FILTER (WHERE status = 2) AS Completed,
              COUNT(*) FILTER (WHERE status = 3) AS Failed
            FROM orders;
        """;

            using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryFirstAsync<OrderSummaryDto>(sql);
        }

        public async Task<IReadOnlyList<OrderRetryMetricDto>> GetRetryDistributionAsync()
        {
            const string sql = """
            SELECT
              retry_count AS RetryCount,
              COUNT(*) AS Count
            FROM orders
            GROUP BY retry_count
            ORDER BY retry_count;
        """;

            using var connection = new NpgsqlConnection(_connectionString);
            return (await connection.QueryAsync<OrderRetryMetricDto>(sql)).ToList();
        }
    }

}
