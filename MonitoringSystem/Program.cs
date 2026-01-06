using MonitoringSystem.Model;
using MonitoringSystem.Repository;
using MonitoringSystem.Utility;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MonitoringSystem")
    .WriteTo.File(
        "logs/monitoringSystem.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
        "[TraceId: {TraceId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    //DI registration goes here
    ComponentRegistration.RegisterComponents(builder.Services);

    builder.Services.AddHealthChecks();
    builder.Services.AddControllers();

    builder.Services.Configure<OrderProcessingOptions>(
    builder.Configuration.GetSection("OrderProcessing"));

    string jaegerUrl = builder.Configuration.GetValue<string>("JaegerUrl") ?? "http://localhost:4317";

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
        .AddService(serviceName: "MonitoringSystem", serviceVersion: "1.0.0"))
        .WithTracing(tracing =>
            {
                tracing
                //.SetSampler(new TraceIdRatioBasedSampler(0.1))
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(jaegerUrl);
                })
                .AddAspNetCoreInstrumentation(opt =>
                {
                    opt.EnrichWithHttpRequest = (activity, request) =>
                    {
                        activity.DisplayName = $"{request.Method} {request.Path}";
                    };
                });
            });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {

    }

    app.UseSerilogRequestLogging(opts =>
    {
        opts.GetLevel = (httpContext, elapsed, ex) =>
        {
            // Customize the log level based on the request path
            if (httpContext.Request.Path.StartsWithSegments("/health"))
                return LogEventLevel.Verbose;

            return LogEventLevel.Information;
        };
    });

    app.MapHealthChecks("/health/live");
    app.MapHealthChecks("/health/ready");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    var connectionString = builder.Configuration.GetConnectionString("OrdersDb")!;
    DatabaseInitializer.Initialize(connectionString);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}


