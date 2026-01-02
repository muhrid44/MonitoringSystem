using OpenTelemetry.Trace;
using Serilog;

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .WriteTo.Console()
           .WriteTo.File("logs/monitoringSystem.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services.AddHealthChecks();
    builder.Services.AddControllers();

    builder.Services.AddOpenTelemetry()
                    .WithTracing(tracing =>
                    {
                        tracing.AddAspNetCoreInstrumentation()
                        .AddConsoleExporter();
                    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
    }

    app.MapHealthChecks("/health/live");
    app.MapHealthChecks("/health/ready");

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

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


