using MassTransit;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.MessageBus.Demo;

public class DemoMessageWorker : BackgroundService
{
    readonly IBus _bus;

    public DemoMessageWorker(IBus bus)
    {
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _bus.Publish(
                new DemoMessage(SomethingNice: $"The time is {DateTimeOffset.Now}"),
                stoppingToken
            );

            await Task.Delay(1000, stoppingToken);
        }
    }
}
