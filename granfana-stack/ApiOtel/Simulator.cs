using FluentResults;

namespace ApiOtel;

public class Simulator
{
    public static Task Delay(int min = 500, int max = 10000) =>
        Task.Delay(TimeSpan.FromMicroseconds(Random.Shared.Next(min, max)));

    public static IResult Response(WeatherForecast weatherForecast)
    {
        var statusCodeList = new int[] { 200, 400, 500 };
        Random.Shared.Shuffle(statusCodeList);

        var status = statusCodeList.FirstOrDefault();
        return status switch
        {
            200 => CustomResults.Ok(weatherForecast),
            400 => CustomResults.BadRequest(new Error("ERROR 400 Family")),
            500 => CustomResults.Problem(new Error("ERROR 500 Family")),
            _ => CustomResults.Problem(new Error("Unmaped error"))
        };
    }

}