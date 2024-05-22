namespace Core.Features.MagicCalculation.BusinessLogic;

public static class MagicCalculator
{
    public static decimal CalculateOutputValue(decimal inputValue)
    {
        var naturalLog = Math.Log(Convert.ToDouble(inputValue));

        var thirdRoot = Math.Pow(naturalLog, 1.0 / 3.0);

        return Convert.ToDecimal(thirdRoot);
    }
}
