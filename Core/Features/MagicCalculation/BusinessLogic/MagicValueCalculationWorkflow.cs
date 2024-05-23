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

    public async Task<MagicValueCalculationWorkflowResult> ExecuteMagicValueCalculationAsync(
        int key,
        decimal inputValue
    )
    {
        var previousOutputValue = await _store.TryGetValueAsync(key);

        if (previousOutputValue is null)
        {
            var defaultValue = CalculationValue.GetDefault(_clock);

            await _store.UpsertValueAsync(key, CalculationValue.GetDefault(_clock));

            return new MagicValueCalculationWorkflowResult(
                computed_value: defaultValue.Value,
                input_value: inputValue,
                previous_value: previousOutputValue?.Value
            );
        }

        var nextValue = previousOutputValue.CalculateNextValue(_clock, inputValue);

        await _store.UpsertValueAsync(
            key,
            previousOutputValue.CalculateNextValue(_clock, inputValue)
        );

        return new MagicValueCalculationWorkflowResult(
            computed_value: nextValue.Value,
            input_value: inputValue,
            previous_value: previousOutputValue?.Value
        );
    }
}
