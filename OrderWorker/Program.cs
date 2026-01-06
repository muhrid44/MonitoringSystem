using MonitoringSystem.Model;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderWorker;
using OrderWorker.Api;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<OrderApiBindingModel>(
    builder.Configuration.GetSection("OrderUrl"));

//registration of OrderApi
builder.Services.AddScoped<IOrderApi, OrderApi>();

string jaegerUrl = builder.Configuration.GetValue<string>("JaegerUrl") ?? "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
    .AddService(serviceName: "OrderWorker", serviceVersion: "1.0.0"))
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

var host = builder.Build();
host.Run();
