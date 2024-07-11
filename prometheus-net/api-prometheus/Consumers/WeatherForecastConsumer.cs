using System.Diagnostics;
using System.Text.Json;

using Api.Prometheus.Controllers;
using Api.Prometheus.CustomMetrics;

using MassTransit;

namespace Api.Prometheus.Consumers;

public class WeatherForecastConsumer(ILogger<WeatherForecastConsumer> logger) : IConsumer<WeatherForecast>
{
    private readonly ILogger<WeatherForecastConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private static readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public async Task Consume(ConsumeContext<WeatherForecast> context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            await Simulator.Delay(max: 10_000);
            if (WeatherForecast.Validate() is false)
            {
                throw new Exception("There's been an error");
            }

            _logger.LogInformation("Message: {0}", JsonSerializer.Serialize(context.Message, options));
        }
        finally
        {
            CustomMetric.Consumer.Observe(stopwatch.ElapsedMilliseconds);
        }
    }
}