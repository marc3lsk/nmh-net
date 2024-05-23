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
                var inputValue = worksheet.Cells[row, 1].Value; // Column A
                var previousOutputValue = worksheet.Cells[row, 2].Value; // Column B
                var outputValue = worksheet.Cells[row, 3].Value; // Column C
                yield return new object[] { inputValue, previousOutputValue, outputValue };
            }
        }
    }

    [Theory]
    [MemberData(
        nameof(GetTestData),
        "./Features/MagicCalculation/BusinessLogic/MagicCalculatorTestData.xlsx",
        MemberType = typeof(MagicCalculatorTests)
    )]
    public void TestWithExcelValues(
        decimal inputValue,
        decimal previousOutputValue,
        decimal outputValue
    )
    {
        Math.Round(MagicCalculator.CalculateOutputValue(inputValue, previousOutputValue), 5)
            .Should()
            .Be(outputValue);
    }

    [Fact]
    public void Zero_Should_Throw_InvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(
            () => MagicCalculator.CalculateOutputValue(0, default)
        );
    }
}
