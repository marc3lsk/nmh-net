using Core.Features.MagicCalculation.BusinessLogic;
using FluentAssertions;
using OfficeOpenXml;

namespace UnitTests.Features.MagicCalculation.BusinessLogic;

public class MagicCalculatorTests
{
    public static IEnumerable<object[]> GetTestData(string excelFilePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (var package = new ExcelPackage(new System.IO.FileInfo(excelFilePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; // Assuming data is in the first sheet
            for (int row = 1; row <= worksheet.Dimension.End.Row; row++) // Skip header row
            {
                var input = worksheet.Cells[row, 1].Value; // Column A
                var output = worksheet.Cells[row, 2].Value; // Column B
                yield return new object[] { input, output };
            }
        }
    }

    [Theory]
    [MemberData(nameof(GetTestData), "./Features/MagicCalculation/BusinessLogic/MagicCalculatorTestData.xlsx", MemberType = typeof(MagicCalculatorTests))]
    public void TestWithExcelValues(double inputValue, double outputValue)
    {
        Math.Round(MagicCalculator.CalculateOutputValue(inputValue), 5).Should().Be(outputValue);
    }

    [Theory]
    [InlineData(0, double.PositiveInfinity)]
    public void Zero_Should_Be_Positive_Infinity(double inputValue, double outputValue)
    {
        Math.Round(MagicCalculator.CalculateOutputValue(inputValue), 5).Should().Be(outputValue);
    }
}
