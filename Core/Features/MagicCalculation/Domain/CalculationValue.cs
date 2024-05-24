using Core.Features.MagicCalculation.BusinessLogic;
using NodaTime;

namespace Core.Features.MagicCalculation.Domain;

public record CalculationValue(decimal Value, Instant UpdatedAt)
{
    public static CalculationValue GetDefault(IClock clock) =>
        new CalculationValue(Value: 2, UpdatedAt: clock.GetCurrentInstant());

    public bool IsExpired(IClock clock) =>
        UpdatedAt.CompareTo(clock.GetCurrentInstant().Minus(Duration.FromSeconds(15))) < 0;

    public CalculationValue CalculateNextValue(IClock clock, decimal inputValue)
    {
        if (inputValue == 0)
        {
            throw new InvalidOperationException("Input value 0 is not allowed");
        }

        if (IsExpired(clock))
            return GetDefault(clock);

        try
        {
            var outputValue = MagicCalculator.CalculateOutputValue(inputValue, Value);

            return new CalculationValue(Value: outputValue, UpdatedAt: clock.GetCurrentInstant());
        }
        catch (OverflowException _)
        {
            // TODO: how to handle this error?
            return GetDefault(clock);
        }
    }
}
