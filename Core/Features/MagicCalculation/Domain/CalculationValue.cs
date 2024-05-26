using Core.Features.MagicCalculation.Helpers;
using NodaTime;

namespace Core.Features.MagicCalculation.Domain;

public record CalculationValue(decimal Value, Instant UpdatedAt)
{
    public const decimal DEFAULT_VALUE = 2;
    public const int EXPIRATION_IN_SECONDS = 15;

    public static CalculationValue GetDefault(IClock clock) =>
        new CalculationValue(Value: DEFAULT_VALUE, UpdatedAt: clock.GetCurrentInstant());

    public bool IsExpired(IClock clock) =>
        UpdatedAt.CompareTo(
            clock.GetCurrentInstant().Minus(Duration.FromSeconds(EXPIRATION_IN_SECONDS))
        ) < 0;

    public CalculationValue CalculateNextValue(IClock clock, decimal inputValue)
    {
        if (inputValue == 0)
        {
            throw new InvalidOperationException("Input value 0 is not allowed");
        }

        if (IsExpired(clock))
            return GetDefault(clock);

        var outputValue = MagicCalculator.CalculateOutputValue(inputValue, Value);

        return new CalculationValue(Value: outputValue, UpdatedAt: clock.GetCurrentInstant());
    }
}
