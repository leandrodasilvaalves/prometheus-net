using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ApiOtel;

public static class CustomResults
{
    private static ILogger Logger { get; set; }

    public static void ConfigureLogger(ILogger logger) =>
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public static IResult ToHttpRespose(this HttpResult result) => result.StatusCode switch
    {
        200 => Ok(result),
        400 => BadRequest(result),
        _ => Problem(result.Error)
    };

    public static IResult Ok(object value)
    {
        Logger.LogInformation(JsonSerializer.Serialize(value));
        return Results.Ok(value);
    }

    public static IResult BadRequest(object error)
    {
        Logger.LogWarning(JsonSerializer.Serialize(error));
        return Results.BadRequest(error);
    }

    public static IResult Problem(Error error) => Problem(error.Message);

    public static IResult Problem(string error)
    {
        Logger.LogError(error);
        var detail = new ProblemDetails
        {
            Title = "Error",
            Detail = error,
            Status = 500,
        };
        return Results.Problem(detail);
    }
}