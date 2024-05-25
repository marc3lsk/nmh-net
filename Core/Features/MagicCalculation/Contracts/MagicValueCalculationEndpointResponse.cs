namespace Core.Features.MagicCalculation.Contracts;

public record MagicValueCalculationEndpointResponse(
    decimal computed_value,
    decimal input_value,
    decimal? previous_value
);
