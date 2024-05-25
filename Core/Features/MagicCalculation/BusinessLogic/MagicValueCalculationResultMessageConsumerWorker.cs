using Abstraction.MessageBus;
using Microsoft.Extensions.Hosting;

namespace Core.Features.MagicCalculation.BusinessLogic;

public class MagicValueCalculationResultMessageConsumerWorker : BackgroundService
{
    IMessageConsumer _messageConsumer;

    public MagicValueCalculationResultMessageConsumerWorker(IMessageConsumer messageConsumer)
    {
        _messageConsumer = messageConsumer;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _messageConsumer.StartConsuming<decimal?>(
            "my-queue-worker",
            msg =>
            {
                Console.WriteLine(
                    $"{nameof(MagicValueCalculationResultMessageConsumerWorker)} received: {msg}"
                );
            },
            cancellationToken
        );

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
