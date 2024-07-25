using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using OpenTelemetry;

namespace ApiOtel;

public static class OtelConfig
{
    public static Meter CustomMeter { get; private set; }
    public static Counter<int> ReceivedRequestsMeter { get; private set; }
    public static ActivitySource ActivitySource { get; private set; }
    public static string ServiceName { get; private set; }

    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        var otelEndpoint = builder.Configuration["OpenTelemetry:Exporter:Otlp:Endpoint"];
        var uriOtel = new Uri(otelEndpoint);
        var serviceName = builder.Configuration["OpenTelemetry:ServiceName"];

        builder.Logging.AddOpenTelemetry(options =>
        {
            options
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName))
                .AddConsoleExporter()
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = uriOtel;
                    opt.ExportProcessorType = ExportProcessorType.Simple;
                });
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;
        });

        _ = builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(serviceName))
        .WithTracing(tracing => tracing
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = uriOtel;
                opt.ExportProcessorType = ExportProcessorType.Simple;
            }))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = uriOtel;
                opt.ExportProcessorType = ExportProcessorType.Simple;
            }));


        CustomMeter = new($"{serviceName}.Api.Otel");
        ReceivedRequestsMeter = CustomMeter.CreateCounter<int>("api_otel_received_requests");
        ActivitySource = new($"{serviceName}.WeatherForecast", "1.0.0");
        ServiceName = serviceName;
    }
    
    public static Activity StartActivity(string name)
    {
        ArgumentNullException.ThrowIfNull(ActivitySource);
        return ActivitySource.StartActivity($"{ServiceName}.{name}");
    }

    public static void ReceivedRequestsInc()
    {
        ArgumentNullException.ThrowIfNull(ReceivedRequestsMeter);
        ReceivedRequestsMeter.Add(1);
    }
}
