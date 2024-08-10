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
    private static Meter CustomMeter { get; set; }
    private static Counter<int> ReceivedRequestsMeter { get; set; }
    private static Histogram<int> IntHistogram { get; set; }
    private static ActivitySource ActivitySource { get; set; }
    private static string ServiceName { get; set; }

    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        var otelEndpoint = builder.Configuration["OpenTelemetry:Exporter:Otlp:Endpoint"];
        var uriOtel = new Uri(otelEndpoint);
        var serviceName = builder.Configuration["OpenTelemetry:ServiceName"];
        ServiceName = serviceName;

        CustomMeter = new($"{serviceName}.Api.Otel", "1.0.0");
        ReceivedRequestsMeter = CustomMeter.CreateCounter<int>("api_otel_received_requests", "req", "Count requests");
        IntHistogram = CustomMeter.CreateHistogram<int>("int_histogram", "number", "Some int histogram");

        ActivitySource = new($"{serviceName}.WeatherForecast", "1.0.0");

        var otel = builder.Services.AddOpenTelemetry();
        ConfigureTracing(otel, serviceName, uriOtel);
        ConfigureMetrics(otel, uriOtel);
        ConfigureLogging(builder.Logging, serviceName, uriOtel);
    }

    public static Activity StartActivity(string name)
    {
        ArgumentNullException.ThrowIfNull(ActivitySource);
        return ActivitySource.StartActivity($"{ServiceName}.{name}");
    }

    public static void ReceivedRequestsInc()
    {
        ArgumentNullException.ThrowIfNull(ReceivedRequestsMeter);
        ReceivedRequestsMeter.Add(1, new KeyValuePair<string, object>("tagName", "tagValue"));
    }

    public static void AddHistogram()
    {
        ArgumentNullException.ThrowIfNull(IntHistogram);
        IntHistogram.Record(Random.Shared.Next(1, 100), new KeyValuePair<string, object>("tagName", "tagValue"));
    }

    public static void AddHistogram(int value)
    {
        ArgumentNullException.ThrowIfNull(IntHistogram);
        IntHistogram.Record(value, new KeyValuePair<string, object>("weatherforecast", value));
    }

    private static void ConfigureLogging(ILoggingBuilder builder, string serviceName, Uri uriOtel)
    {
        builder.AddOpenTelemetry(options =>
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
    }

    private static void ConfigureTracing(OpenTelemetryBuilder otel, string serviceName, Uri uriOtel)
    {
        otel.ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing => tracing
            .AddSource(serviceName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = uriOtel;
                opt.ExportProcessorType = ExportProcessorType.Simple;
            }));
    }

    private static void ConfigureMetrics(OpenTelemetryBuilder otel, Uri uriOtel)
    {
        otel.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddMeter(CustomMeter.Name)
            .AddConsoleExporter()
            .AddOtlpExporter(opt =>
            {
                opt.Endpoint = uriOtel;
                opt.ExportProcessorType = ExportProcessorType.Simple;
            }));
    }
}
