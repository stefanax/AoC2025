using System.Linq;
using System.Numerics;

namespace AoC2024;

public class Day6
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(6, false);
        var inputList = _inputFiles.SplitString(input)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToList();

        if (inputList.Count < 2)
        {
            throw new InvalidOperationException("Input must include at least one row of values and an operator row.");
        }

        var columnCount = inputList[0].Length;

        if (inputList.Any(row => row.Length != columnCount))
        {
            throw new InvalidOperationException("All rows must have the same number of columns.");
        }

        var operatorRow = inputList[^1];
        var valueRows = inputList[..^1];

        BigInteger total = 0;

        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            var operationSymbol = operatorRow[columnIndex];
            var isAddition = operationSymbol == "+";
            var isMultiplication = operationSymbol == "*";

            if (!isAddition && !isMultiplication)
            {
                throw new InvalidOperationException($"Unexpected operator '{operationSymbol}' in column {columnIndex}.");
            }

            var columnValue = isAddition ? BigInteger.Zero : BigInteger.One;

            foreach (var row in valueRows)
            {
                var value = BigInteger.Parse(row[columnIndex]);
                columnValue = isAddition ? columnValue + value : columnValue * value;
            }

            total += columnValue;
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(total);
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(6, false);
        var inputList = _inputFiles.SplitString(input);

        Console.WriteLine("Step two result:");
    }
}
