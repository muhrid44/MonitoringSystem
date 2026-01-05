using MonitoringSystem.Model;

namespace MonitoringSystem.Repository
{
    public interface IOrderMetricsRepository
    {
        Task<OrderSummaryDto> GetOrderSummaryAsync();
        Task<IReadOnlyList<OrderRetryMetricDto>> GetRetryDistributionAsync();
    }

}
