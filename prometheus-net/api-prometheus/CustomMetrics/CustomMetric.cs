using Prometheus;

namespace Api.Prometheus.CustomMetrics;

public static class CustomMetric
{
    public static readonly Histogram Endpoint = Metrics.CreateHistogram(
        "weatherforecast_endpoint_response_time", string.Empty, new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(0, 2, 10),
        });

    public static readonly Histogram Consumer = Metrics.CreateHistogram(
        "weatherforecast_consumer_processing_time", string.Empty, new HistogramConfiguration
        {
            Buckets = Histogram.PowersOfTenDividedBuckets(0, 2, 10),
        });
}
