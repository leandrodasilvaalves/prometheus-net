using Prometheus;

namespace Api.Prometheus.CustomMetrics;

public static class RegisterWeatherForeCastMetrics
{
    public static readonly Counter Success = Metrics.CreateCounter(
        "register_weatherforecast_success", string.Empty);

    public static readonly Counter Error = Metrics.CreateCounter(
        "register_weatherforecast_error", string.Empty);

    public static readonly Histogram ResponseTime = Metrics.CreateHistogram(
        "register_weatherforecast_time_response", string.Empty, new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(0, 2, 10)
        });
}

public static class ListWeatherForecast
{
    public static readonly Counter Success = Metrics.CreateCounter(
        "list_weatherforecast_success", string.Empty);

    public static readonly Counter Error = Metrics.CreateCounter(
        "list_weatherforecast_error", string.Empty);

    public static readonly Histogram ResponseTime = Metrics.CreateHistogram(
        "list_weatherforecast_time_response", string.Empty, new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(0, 2, 10),
        });
}
