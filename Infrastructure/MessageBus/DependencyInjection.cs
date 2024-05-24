using Infrastructure.MessageBus.Demo;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MessageBus;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<DemoMessageConsumer>();

            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    EndpointConvention.Map<DemoMessage>(new Uri("queue:my-queue"));

                    cfg.ReceiveEndpoint(
                        "my-queue",
                        e =>
                        {
                            e.ConfigureConsumer<DemoMessageConsumer>(context);
                        }
                    );
                }
            );
        });

        services.AddHostedService<DemoMessageWorker>();

        return services;
    }
}
