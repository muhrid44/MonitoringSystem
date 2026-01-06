using MonitoringSystem.Model;
using System.Net.Http.Json;

namespace OrderWorker.Api
{
    public class OrderApi : IOrderApi
    {
        private readonly HttpClient _http;
        private readonly OrderApiBindingModel _orderApi;

        public OrderApi(HttpClient http, OrderApiBindingModel orderApiBindingModel)
        {
            _http = http;
            _orderApi = orderApiBindingModel;
        }

        public async Task<IReadOnlyList<Guid>> GetPendingOrdersAsync()
        {
            var orders = await _http.GetFromJsonAsync<IReadOnlyList<Guid>>(
                $"{_orderApi.OrderUrl}/get-pending-order-ids");

            return orders ?? Array.Empty<Guid>();
        }

        public async Task UpdateOrderStatus(Guid orderId, CancellationToken ct)
        {
            var response = await _http.PatchAsync(
                            $"{_orderApi.OrderUrl}/{orderId}/process",
                            content: null,
                            cancellationToken: ct);

            response.EnsureSuccessStatusCode();
        }
    }
}
