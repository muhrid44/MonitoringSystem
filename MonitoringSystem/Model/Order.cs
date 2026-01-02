namespace MonitoringSystem.Model
{
    public class Order
    {
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Status { get; private set; }

        public Order()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "Created";
        }

        public void MarkProcessing() => Status = "Processing";
        public void MarkFailed() => Status = "Failed";
        public void MarkCompleted() => Status = "Completed";
    }
}
