using Prometheus;

namespace Api.Prometheus.CustomMetrics;

public static class CustomMetricsSamples
{
    public static WebApplication RunCounterMetrics(this WebApplication app)
    {
        var recordsProcessed = Metrics.CreateCounter("sample_records_processed_total", "Total number of records processed.");
        _ = Task.Run(async delegate
        {
            while (true)
            {
                recordsProcessed.Inc();

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        });

        return app;
    }
}