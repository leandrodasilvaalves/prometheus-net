using System.Text.Json;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiOtel;

public static class CustomResults
{
    private static ILogger Logger { get; set; }

    public static void ConfigureLogger(ILogger logger) =>
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public static IResult Ok(object value)
    {
        var result = Result.Ok(value);
        Logger.LogInformation(JsonSerializer.Serialize(result));
        return Results.Ok(result);
    }

    public static IResult BadRequest(IError error)
    {
        var result = Result.Fail(error);
        Logger.LogWarning(JsonSerializer.Serialize(result));
        return Results.BadRequest(result);
    }

    public static IResult Problem(IError error)
    {
        var result = JsonSerializer.Serialize(error);
        Logger.LogError(result);

        var detail = new ProblemDetails
        {
            Title = "Error",
            Detail = result
        };
        return Results.Problem(detail);
    }
}