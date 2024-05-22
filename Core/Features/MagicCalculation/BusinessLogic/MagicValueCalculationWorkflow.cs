using Abstraction.KeyValueStore;
using Core.Features.MagicCalculation.Domain;
using NodaTime;

namespace Core.Features.MagicCalculation.BusinessLogic;

public class MagicValueCalculationWorkflow
{
    IClock _clock;
    IKeyValueStore<int, CalculationValue> _store;

    public MagicValueCalculationWorkflow(IClock clock, IKeyValueStore<int, CalculationValue> store)
    {
        _clock = clock;
        _store = store;
    }

    public async Task ExecuteMagicValueCalculationAsync(int key, decimal inputValue)
    {
        if (inputValue == 0)
        {
            throw new InvalidOperationException("Input value 0 is not allowed");
        }

        var previousOutputValue = await _store.TryGetValueAsync(key);

        if (previousOutputValue is null)
        {
            await UpsertDefaultValueAsync(key);
            return;
        }

        if (previousOutputValue.IsExpired(_clock))
        {
            await UpsertDefaultValueAsync(key);
            return;
        }

        var outputValue = MagicCalculator.CalculateOutputValue(
            inputValue,
            previousOutputValue.Value
        );

        await _store.UpsertValueAsync(
            key,
            new CalculationValue(Value: outputValue, UpdatedAt: _clock.GetCurrentInstant())
        );
    }

    async Task UpsertDefaultValueAsync(int key)
    {
        await _store.UpsertValueAsync(key, CalculationValue.GetDefault(_clock));
    }
}
