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
                    opt.Endpoint = new Uri("http://localhost:4317");
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
        var seedCounts = builder.Configuration.GetRequiredSection("SeedSample")!.Value;

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                                         .CreateLogger("Seeder");

        await DevelopmentSeedingDataDummy.SeedAsync(
            app.Services,
            logger,
            int.Parse(seedCounts!)
            );
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


