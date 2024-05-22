using NodaTime;

namespace Core.Features.MagicCalculation.Domain;

public record CalculationValue(
    decimal Value,
    Instant UpdatedAt
);
