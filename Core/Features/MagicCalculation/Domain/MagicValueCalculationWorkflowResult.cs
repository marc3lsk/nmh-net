namespace Core.Features.MagicCalculation.Domain;

public record MagicValueCalculationWorkflowResult(
    decimal computed_value,
    decimal input_value,
    decimal? previous_value
);
