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
        var lines = _inputFiles.SplitString(input)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (lines.Count < 2)
        {
            throw new InvalidOperationException("Input must include at least one row of values and an operator row.");
        }

        var columnCount = lines.Max(line => line.Length);
        var operatorRow = lines[^1].PadRight(columnCount, ' ');
        var valueRows = lines[..^1]
            .Select(line => line.PadRight(columnCount, ' '))
            .ToList();

        BigInteger total = 0;

        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
        {
            var operationSymbol = operatorRow[columnIndex];
            var isAddition = operationSymbol == '+';
            var isMultiplication = operationSymbol == '*';

            if (!isAddition && !isMultiplication)
            {
                throw new InvalidOperationException($"Unexpected operator '{operationSymbol}' in column {columnIndex}.");
            }

            var subColumnValues = new List<BigInteger>();
            var isBuildingNumber = false;

            foreach (var valueRow in valueRows)
            {
                var character = valueRow[columnIndex];

                if (!char.IsDigit(character))
                {
                    isBuildingNumber = false;
                    continue;
                }

                if (!isBuildingNumber)
                {
                    subColumnValues.Add(new BigInteger(character - '0'));
                    isBuildingNumber = true;
                    continue;
                }

                subColumnValues[^1] = subColumnValues[^1] * 10 + (character - '0');
            }

            var columnResult = isAddition
                ? subColumnValues.Aggregate(BigInteger.Zero, (current, value) => current + value)
                : subColumnValues.Aggregate(BigInteger.One, (current, value) => current * value);

            total += columnResult;
        }

        Console.WriteLine("Step two result:");
        Console.WriteLine(total);
    }
}
