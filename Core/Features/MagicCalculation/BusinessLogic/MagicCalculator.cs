namespace Core.Features.MagicCalculation.BusinessLogic;

public static class MagicCalculator
{
    public static double CalculateOutputValue(double inputValue)
    {
        var naturalLog = Math.Log(inputValue);

        var thirdRoot = Math.Pow(naturalLog, 1.0 / 3.0);

        return thirdRoot;
    }
}
