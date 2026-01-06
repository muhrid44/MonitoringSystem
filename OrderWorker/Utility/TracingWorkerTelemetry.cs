using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OrderWorker.Utility
{
    public static class TracingWorkerTelemetry
    {
        public const string ServiceName = "OrderWorker";

        public static readonly ActivitySource ActivitySource =
            new(ServiceName);
    }
}
