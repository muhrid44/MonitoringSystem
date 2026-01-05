namespace MonitoringSystem.Model
{
    public sealed class OrderSummaryDto
    {
        public int Total { get; init; }
        public int Created { get; init; }
        public int Processing { get; init; }
        public int Completed { get; init; }
        public int Failed { get; init; }
    }

}
