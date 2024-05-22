using NodaTime;

namespace Core.Features.MagicCalculation.Domain;

public record CalculationValue(decimal Value, Instant UpdatedAt)
{
    public static CalculationValue GetDefault(IClock clock) =>
        new CalculationValue(Value: 2, UpdatedAt: clock.GetCurrentInstant());

    public bool IsExpired(IClock clock) =>
        UpdatedAt.CompareTo(clock.GetCurrentInstant().Minus(Duration.FromSeconds(15))) < 0;
}
