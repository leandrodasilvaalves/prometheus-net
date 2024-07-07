using System.Diagnostics;

using Api.Prometheus.Consumers;
using Api.Prometheus.CustomMetrics;

using MassTransit;

using Microsoft.AspNetCore.Mvc;

namespace Api.Prometheus.Controllers;

[ApiController]
[Route("api/weatherforecast")]
public class WeatherForecastController : ControllerBase
{
    public readonly IPublishEndpoint _publishEndpoint;
    private readonly RabbitConfig _rabbitConfig;

    public WeatherForecastController(IPublishEndpoint publishEndpoint,
                                     RabbitConfig rabbitConfig)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _rabbitConfig = rabbitConfig ?? throw new ArgumentNullException(nameof(rabbitConfig));
    }

    public List<WeatherForecast> Data = [];

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (WeatherForecast.Validate() is false)
                throw new Exception("There's been an error");

            var forecast = Enumerable
                                .Range(1, 5)
                                .Select(WeatherForecast.Generate)
                                .ToArray();

            if (_rabbitConfig.Enabled)
            {
                await _publishEndpoint.Publish(forecast.FirstOrDefault());
            }

            ListWeatherForecast.Success.Inc();
            ListWeatherForecast.ResponseTime.Observe(stopwatch.ElapsedMilliseconds);
            return Ok(forecast);
        }
        catch (Exception)
        {
            ListWeatherForecast.Error.Inc();
            return new StatusCodeResult(500);
        }
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] WeatherForecast request)
    {
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (WeatherForecast.Validate() is false)
                throw new Exception("There's been an error");

            Data.Add(request);
            if (_rabbitConfig.Enabled)
            {
                await _publishEndpoint.Publish(request);
            }
            RegisterWeatherForeCastMetrics.Success.Inc();
            RegisterWeatherForeCastMetrics.ResponseTime.Observe(stopwatch.ElapsedMilliseconds);
            return Ok(request);
        }
        catch (Exception)
        {
            RegisterWeatherForeCastMetrics.Error.Inc();
            return new StatusCodeResult(500);
        }
    }

}

public record WeatherForecast(DateTime Date, int TemperatureC, string Summary)
{
    private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public static bool Validate() => DateTime.UtcNow.Second % 2 == 0;

    public static WeatherForecast Generate(int addDays = 0) => new(
        DateTime.Now.AddDays(addDays),
        Random.Shared.Next(-20, 55),
        Summaries[Random.Shared.Next(Summaries.Length)]
    );
}