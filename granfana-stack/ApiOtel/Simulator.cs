
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
            200 => CustomResults.Ok(new HttpResult(weatherForecast)),
            400 => CustomResults.BadRequest(new HttpResult(400, Error.BadRequest)),
            500 => CustomResults.Problem(Error.InternalError),
            _ => CustomResults.Problem("Unmaped error"),
        };
    }
}