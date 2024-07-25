using System.Net;

namespace ApiOtel;

public class HttpResult
{
    public HttpResult() { }

    public HttpResult(WeatherForecast value)
    {
        StatusCode = 200;
        Value = value;
    }

    public HttpResult(int statusCode, Error error)
    {
        StatusCode = statusCode;
        Error = error;
    }

    public int StatusCode { get; set; }
    public WeatherForecast Value { get; set; }
    public Error Error { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode <= 299;

    public static HttpResult BadRequest(Error error = null) =>
        new((int)HttpStatusCode.BadRequest, error ?? Error.BadRequest);

    public static HttpResult InternalError(Error error = null) =>
        new((int)HttpStatusCode.InternalServerError, error ?? Error.InternalError);

}

public class Error
{
    public static Error BadRequest = new() { Message = "Error 400  family" };
    public static Error InternalError = new() { Message = "Error 500  family" };
    public string Message { get; set; }
}
