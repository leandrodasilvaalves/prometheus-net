using ApiOtel;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration.GetSection("OpenTelemetry:ServiceName").Value;

var otelEndpoint = builder.Configuration
    .GetSection("OpenTelemetry:Exporter:Otlp:Endpoint").Value;
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

CustomResults.ConfigureLogger(app.Logger);

app.MapGet("/weatherforecast", async ([FromServices] ILogger<Program> logger) =>
{
    await Simulator.Delay();
    return Simulator.Response(WeatherForecast.Generate());
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();