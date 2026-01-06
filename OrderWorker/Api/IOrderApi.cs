using System;
using System.Collections.Generic;
using System.Text;

namespace OrderWorker.Api
{
    public interface IOrderApi
    {
        Task<IReadOnlyList<Guid>> GetPendingOrdersAsync();
        Task UpdateOrderStatus(Guid orderId, CancellationToken ct);
    }
}
