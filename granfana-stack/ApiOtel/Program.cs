using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

const string serviceName = "api-otel";
var otelEndpoint = builder.Configuration
    .GetSection("OpenTelemetry:Exporter:Otlp:Endpoint").Get<string>();
var uriOtel = new Uri(otelEndpoint);

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
            opt.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
        });
    options.IncludeFormattedMessage = true;
    options.ParseStateValues = true;
});

builder.Services.AddOpenTelemetry()
      .ConfigureResource(resource => resource.AddService(serviceName))
      .WithTracing(tracing => tracing
          .AddSource(serviceName)
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(opt =>
          {
              opt.Endpoint = uriOtel;
              opt.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
          }))
      .WithMetrics(metrics => metrics
          .AddAspNetCoreInstrumentation()
          .AddConsoleExporter()
          .AddOtlpExporter(opt =>
          {
              opt.Endpoint = uriOtel;
              opt.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
          }));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapGet("/weatherforecast", ([FromServices] ILogger<Program> logger) =>
{
    logger.LogInformation("Any information {0}", Guid.NewGuid());
    logger.LogWarning("Any warnging {0}", Guid.NewGuid());

    var forecast = Enumerable.Range(1, 5)
        .Select(index =>
        {
            var weatherforecast = WeatherForecast.Generate(index);
            logger.LogError("Something very bad happened: {0}", JsonSerializer.Serialize(weatherforecast));
            return weatherforecast;
        })
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();