namespace Abstraction.MessageBus;

public interface IMessagePublisher
{
    void Publish<T>(string queueName, T message);
}
