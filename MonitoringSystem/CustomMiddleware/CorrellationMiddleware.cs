namespace MonitoringSystem.CustomMiddleware
{
    public class CorrellationMiddleware
    {
        private const string Header = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrellationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(Header, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Items[Header] = correlationId;
            context.Response.Headers[Header] = correlationId;

            await _next(context);
        }
    }
}
