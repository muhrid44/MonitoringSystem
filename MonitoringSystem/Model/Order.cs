namespace MonitoringSystem.Model
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public void MarkProcessing() => Status = OrderStatus.Processing;
        public void MarkFailed() => Status = OrderStatus.Failed;
        public void MarkCompleted() => Status = OrderStatus.Completed;

        public enum OrderStatus
        {
            Created = 0,
            Processing = 1,
            Completed = 2,
            Failed = 3
        }
    }
}
