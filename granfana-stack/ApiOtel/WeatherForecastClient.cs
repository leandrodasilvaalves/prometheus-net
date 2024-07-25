
using System.Text.Json;

namespace ApiOtel;

public class WeatherForecastClient(HttpClient httpClient, ILogger<WeatherForecastClient> logger) : IWeatherForecastClient
{
    private readonly ILogger<WeatherForecastClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

    public async Task<HttpResult> GetAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/weatherforecast");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HttpResult>();
        }
        catch (HttpRequestException exception)
        {
            return TranslateError(exception);
        }
    }

    public HttpResult TranslateError(HttpRequestException exception)
    {
        if (exception.Message.Contains("400"))
        {
            _logger.LogWarning("Warning: {0}, {1}", exception.Message, JsonSerializer.Serialize(exception.StackTrace));
            return HttpResult.BadRequest();
        }

        _logger.LogError("Error: {0}, {1}", exception.Message, JsonSerializer.Serialize(exception.StackTrace));
        return HttpResult.InternalError();
    }
}


public interface IWeatherForecastClient
{
    public Task<HttpResult> GetAsync();
}