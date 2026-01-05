namespace MonitoringSystem.Model
{
    public class OrderProcessingOptions
    {
        public int MaxRetryCount { get; init; } = 3;
        public double FailureRate { get; init; } = 0.2;
    }

}
