using Core.Features.MagicCalculation.Contracts;
using MassTransit;

namespace Core.Features.MagicCalculation.MessageConsumers;

public class MagicValueCalculationResultMessageConsumer
    : IConsumer<MagicValueCalculationResultMessage>
{
    readonly IBus _bus;

    public MagicValueCalculationResultMessageConsumer(IBus bus)
    {
        _bus = bus;
    }

    public Task Consume(ConsumeContext<MagicValueCalculationResultMessage> context)
    {
        Console.WriteLine(
            $"{nameof(MagicValueCalculationResultMessageConsumer)} received: {context.Message.computed_value}"
        );
        return Task.CompletedTask;
    }
}
