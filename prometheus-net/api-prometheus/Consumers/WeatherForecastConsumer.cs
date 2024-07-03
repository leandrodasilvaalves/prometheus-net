using System.Text.Json;
using Api.Prometheus.Controllers;
using MassTransit;

namespace Api.Prometheus.Consumers;

public class WeatherForecastConsumer(ILogger<WeatherForecastConsumer> logger) : IConsumer<WeatherForecast>
{
    private readonly ILogger<WeatherForecastConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private static readonly JsonSerializerOptions options = new() { WriteIndented = true };

    public async Task Consume(ConsumeContext<WeatherForecast> context)
    {
        await Task.Delay(TimeSpan.FromSeconds(6));
        _logger.LogInformation("Message: {0}", JsonSerializer.Serialize(context.Message, options));
    }
}

public class RabbitConfig
{
    public string Host { get; set; }
    public string VHost { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }
    public bool Enabled { get; set; }
    public string ReceiveEndpoint { get; set; }
};

public static class MassTransitExtensions
{
    public static void ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var config = new RabbitConfig();
        configuration.GetSection("Rabbit").Bind(config);
        services.AddSingleton(config);

        services.AddMassTransit(x =>
        {
            if (config.Enabled)
            {
                x.AddConsumer<WeatherForecastConsumer>();
            }

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config.Host, config.VHost, h =>
                {
                    h.Username(config.User);
                    h.Password(config.Pass);
                });

                if (config.Enabled)
                {
                    cfg.ReceiveEndpoint(config.ReceiveEndpoint, endpoint =>
                    {
                        endpoint.ConfigureConsumer<WeatherForecastConsumer>(context, consumer =>
                            consumer.UseMessageRetry(retry =>
                                retry.Interval(5, TimeSpan.FromSeconds(2))));
                    });
                }

                cfg.ConfigureEndpoints(context);
            });
        });
    }

}