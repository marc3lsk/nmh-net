namespace Abstraction.MessageBus;

public interface IMessageConsumer
{
    Task StartConsuming(
        string queueName,
        Action<string> onMessageReceived,
        CancellationToken cancellationToken
    );
}
