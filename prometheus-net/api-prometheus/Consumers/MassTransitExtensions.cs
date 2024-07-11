using MassTransit;

namespace Api.Prometheus.Consumers;

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

public class RabbitConfig
{
    public string Host { get; set; }
    public string VHost { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }
    public bool Enabled { get; set; }
    public string ReceiveEndpoint { get; set; }
};
