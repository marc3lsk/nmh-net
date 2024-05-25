namespace Abstraction.MessageBus;

public interface IMessageConsumer
{
    Task StartConsuming<T>(
        string queueName,
        Action<T?> onMessageReceived,
        CancellationToken cancellationToken
    );
}
