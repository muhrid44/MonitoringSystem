namespace MonitoringSystem.Utility
{
    public class OrderPersistenceException : Exception
    {
        public OrderPersistenceException(string message, Exception inner)
        : base(message, inner) { }
    }
}
