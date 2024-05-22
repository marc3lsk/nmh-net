using Core.Features.MagicCalculation.Domain;
using FluentAssertions;
using Infrastructure.KeyValueStore;
using NodaTime.Testing;

namespace UnitTests.Inrastructure.KeyValueStore;

public class KeyValueStoreInMemoryTests
{
    [Fact]
    public async Task It_Works()
    {
        var clock = FakeClock.FromUtc(0, 01, 01, 00, 00, 00);
        var store = new KeyValueStoreInMemory<int, CalculationValue>();

        var key = 1;

        var value = new CalculationValue(Value: 2, UpdatedAt: clock.GetCurrentInstant());

        await store.UpsertValueAsync(key, value);

        (await store.TryGetValueAsync(key)).Should().Be(value);
    }

    [Fact]
    public async Task Not_Found()
    {
        var store = new KeyValueStoreInMemory<int, CalculationValue>();
        (await store.TryGetValueAsync(0)).Should().BeNull();
    }
}
