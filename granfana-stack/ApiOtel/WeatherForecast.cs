namespace ApiOtel;

public class WeatherForecast(DateTime date, int temperatureC, string summary)
{
    private static readonly string[] Summaries =
    [
        "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching"
    ];

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public DateTime Date { get; } = date;
    public int TemperatureC { get; } = temperatureC;
    public string Summary { get; } = summary;

    public static bool Validate() => DateTime.UtcNow.Second % 2 == 0;

    public static WeatherForecast Generate(int addDays = 0) => new(
        DateTime.Now.AddDays(addDays),
        Random.Shared.Next(-20, 55),
        Summaries[Random.Shared.Next(Summaries.Length)]
    );
}