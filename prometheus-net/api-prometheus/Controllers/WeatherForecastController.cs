using System.Diagnostics;
using Api.Prometheus.CustomMetrics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Prometheus.Controllers;

[ApiController]
[Route("api/weatherforecast")]
public class WeatherForecastController : ControllerBase
{
    private readonly string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public List<WeatherForecast> Data = [];

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (Validate() is false)
                throw new Exception("There's been an error");

            var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateTime.Now.AddDays(index),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
            .ToArray();

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
    public IActionResult Post([FromBody] WeatherForecast request)
    {
        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (Validate() is false)
                throw new Exception("There's been an error");

            Data.Add(request);
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

    private static bool Validate() => DateTime.UtcNow.Second % 2 == 0;
}

public record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}