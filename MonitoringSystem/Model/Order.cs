namespace MonitoringSystem.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public int RetryCount { get; private set; }
        public void MarkProcessing() => Status = OrderStatus.Processing;
        public void MarkCompleted() => Status = OrderStatus.Completed;
        public void MarkCreated() => Status = OrderStatus.Created;
        public void IncrementRetry() => RetryCount++;

        public enum OrderStatus
        {
            Created = 0,
            Processing = 1,
            Completed = 2,
            Failed = 3
        }

        public void MarkFailed(int maxRetry)
        {
            RetryCount++;
            Status = RetryCount >= maxRetry
                ? OrderStatus.Failed
                : OrderStatus.Created;
        }
    }
}
