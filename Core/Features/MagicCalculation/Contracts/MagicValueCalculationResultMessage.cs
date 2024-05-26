namespace Core.Features.MagicCalculation.Contracts;

public record MagicValueCalculationResultMessage(
    decimal ComputedValue,
    decimal InputValue,
    decimal? PreviousValue
);
