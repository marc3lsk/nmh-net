using MassTransit;

namespace Infrastructure.MessageBus.Demo;

public class DemoMessageConsumer : IConsumer<DemoMessage>
{
    readonly IBus _bus;

    public DemoMessageConsumer(IBus bus)
    {
        _bus = bus;
    }

    public Task Consume(ConsumeContext<DemoMessage> context)
    {
        Console.WriteLine(
            $"{nameof(DemoMessageConsumer)} success with a nice message: {context.Message.SomethingNice}"
        );
        return Task.CompletedTask;
    }
}
