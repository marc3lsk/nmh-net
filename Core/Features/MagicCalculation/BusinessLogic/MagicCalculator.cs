namespace Core.Features.MagicCalculation.BusinessLogic;

public static class MagicCalculator
{
    public static decimal CalculateOutputValue(decimal inputValue, decimal? previousOutputValue)
    {
        if (inputValue == 0)
            throw new InvalidOperationException("Input value 0 is not allowed");

        if (previousOutputValue is null)
            return 2;

        var naturalLog = Math.Log(Convert.ToDouble(inputValue / previousOutputValue));

        var isNegative = naturalLog < 0;

        var thirdRoot = Math.Pow(Math.Abs(naturalLog), 1.0 / 3.0);

        return Convert.ToDecimal(isNegative ? -1 * thirdRoot : thirdRoot);
    }
}
