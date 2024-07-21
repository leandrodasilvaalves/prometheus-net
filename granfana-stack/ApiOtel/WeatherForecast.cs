namespace ApiOtel;

public class WeatherForecast
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

    public WeatherForecast() { }

    public WeatherForecast(DateTime date, int temperatureC, string summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Summary { get; set; }

    public static bool Validate() => DateTime.UtcNow.Second % 2 == 0;

    public static WeatherForecast Generate(int addDays = 0) => new(
        DateTime.Now.AddDays(addDays),
        Random.Shared.Next(-20, 55),
        Summaries[Random.Shared.Next(Summaries.Length)]
    );
}