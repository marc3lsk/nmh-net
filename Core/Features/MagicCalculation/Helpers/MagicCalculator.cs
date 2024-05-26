namespace Core.Features.MagicCalculation.Helpers;

public static class MagicCalculator
{
    public static decimal CalculateOutputValue(decimal inputValue, decimal previousOutputValue)
    {
        if (inputValue == 0)
            throw new InvalidOperationException("Input value 0 is not allowed");

        var naturalLog = Math.Log(Convert.ToDouble(inputValue / previousOutputValue));

        var isNegative = naturalLog < 0;

        // Math.Pow does not know how to handle cube root of negative number,
        // but according to online article and test excel values,
        // we should simply compute cube root of abs number and then flip to negative
        // https://study.com/skill/learn/how-to-find-the-cube-root-of-a-negative-number-explanation.html

        var cubeRoot = Math.Pow(Math.Abs(naturalLog), 1.0 / 3.0);

        return Convert.ToDecimal(isNegative ? -1 * cubeRoot : cubeRoot);
    }
}
