using Abstraction.MessageBus;
using Core.Features.MagicCalculation.BusinessLogic;
using Core.Features.MagicCalculation.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.MessageBus;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        // Config
        services.AddOptions<RabbitMqConfig>().BindConfiguration(nameof(RabbitMqConfig));
        services.AddTransient(sp => sp.GetRequiredService<IOptions<RabbitMqConfig>>().Value);

        // MassTransit version

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MagicValueCalculationResultMessageConsumer>();

            x.UsingRabbitMq(
                (context, cfg) =>
                {
                    EndpointConvention.Map<MagicValueCalculationResultMessage>(
                        new Uri("queue:my-queue")
                    );

                    cfg.ReceiveEndpoint(
                        "my-queue",
                        e =>
                        {
                            e.ConfigureConsumer<MagicValueCalculationResultMessageConsumer>(
                                context
                            );
                        }
                    );
                }
            );
        });

        // Custom low level RabbitMQ version

        services.AddSingleton<IMessagePublisher>(sp => new RabbitMqPublisher(
            sp.GetRequiredService<RabbitMqConfig>().HostName
        ));
        services.AddSingleton<IMessageConsumer>(sp => new RabbitMqConsumer(
            sp.GetRequiredService<RabbitMqConfig>().HostName
        ));

        return services;
    }
}
