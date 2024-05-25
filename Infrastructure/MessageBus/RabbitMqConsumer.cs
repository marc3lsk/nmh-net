using System.Text;
using Abstraction.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.MessageBus;

public class RabbitMqConsumer : IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqConsumer(string hostname)
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task StartConsuming<T>(
        string queueName,
        Action<T?> onMessageReceived,
        CancellationToken cancellationToken
    )
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        var _consumer = new EventingBasicConsumer(_channel);
        _consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            onMessageReceived(JsonConvert.DeserializeObject<T>(message));
        };

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: _consumer);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
