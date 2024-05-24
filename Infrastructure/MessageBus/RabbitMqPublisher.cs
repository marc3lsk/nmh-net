using System.Text;
using Abstraction.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Infrastructure.MessageBus;

internal class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqPublisher(string hostname)
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(string queueName, T message)
    {
        _channel.QueueDeclare(
            queue: queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        _channel.BasicPublish(
            exchange: "",
            routingKey: queueName,
            basicProperties: null,
            body: body
        );
    }
}
