using ApiOtel;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var clientMode = builder.Configuration["Mode"] == "CLIENT";

builder.ConfigureOpenTelemetry();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (clientMode)
{
    builder.Services.
        AddHttpClient<IWeatherForecastClient, WeatherForecastClient>(client =>
            client.BaseAddress = new Uri(builder.Configuration["Client:Url"]));
}

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

CustomResults.ConfigureLogger(app.Logger);

app.MapGet("/weatherforecast", async ([FromServices] IWeatherForecastClient client,
                                      [FromServices] ILogger<Program> logger) =>
{
    using var activity = OtelConfig.StartActivity("ApiOtelActiviy");
    logger.LogInformation("Some log using opentelemtry");
    OtelConfig.ReceivedRequestsInc();
    OtelConfig.AddHistogram();

    await Simulator.Delay();
    if (clientMode)
    {
        logger.LogWarning("Client Mode activated");
        activity?.SetTag("test-1", "Hello World!");
        activity?.SetTag("foo", "bar");

        var result = await client.GetAsync();
        return result.ToHttpRespose();
    }
    return Simulator.Response(WeatherForecast.Generate());

})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/weatherforecast", (int value)=>
{
    OtelConfig.AddHistogram(value);
    return Results.Accepted();
})
.WithName("PostWeatherForecast")
.WithOpenApi();

app.Run();

