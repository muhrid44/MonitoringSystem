using System.Diagnostics;

namespace MonitoringSystem.Utility
{
    public static class TracingSystemTelemetry
    {
        public const string ServiceName = "OrderWorker";

        public static readonly ActivitySource ActivitySource =
            new(ServiceName);
    }
}
