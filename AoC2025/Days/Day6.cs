using System.Linq;
using System.Numerics;
using System.Text;

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
        var lines = _inputFiles.SplitString(input).ToList();

        if (lines.Count == 0)
        {
            throw new InvalidOperationException("Input must include at least one row.");
        }

        var columnCount = lines.Max(line => line.Length);
        var paddedLines = lines
            .Select(line => line.PadRight(columnCount, ' '))
            .ToList();

        BigInteger total = 0;
        var currentNumbers = new List<BigInteger>();
        char? currentOperator = null;

        void FinalizeGroup()
        {
            if (currentNumbers.Count == 0)
            {
                return;
            }

            if (currentOperator is null)
            {
                throw new InvalidOperationException("Encountered a number group without an operator.");
            }

            var result = currentOperator == '+'
                ? currentNumbers.Aggregate(BigInteger.Zero, (running, value) => running + value)
                : currentNumbers.Aggregate(BigInteger.One, (running, value) => running * value);

            total += result;
            currentNumbers.Clear();
            currentOperator = null;
        }

        for (var columnIndex = columnCount - 1; columnIndex >= 0; columnIndex--)
        {
            var columnBuilder = new StringBuilder();
            var isBlankColumn = true;

            foreach (var line in paddedLines)
            {
                var character = line[columnIndex];
                columnBuilder.Append(character);

                if (character != ' ')
                {
                    isBlankColumn = false;
                }
            }

            if (isBlankColumn)
            {
                FinalizeGroup();
                continue;
            }

            var columnData = new string(columnBuilder.ToString()
                .Where(character => character != ' ')
                .ToArray());

            if (columnData.Length == 0)
            {
                FinalizeGroup();
                continue;
            }

            char? operationSymbol = null;

            if (columnData[^1] == '+' || columnData[^1] == '*')
            {
                operationSymbol = columnData[^1];
                columnData = columnData[..^1];
            }

            if (columnData.Length > 0)
            {
                var columnValue = BigInteger.Parse(columnData);
                currentNumbers.Add(columnValue);
            }

            if (operationSymbol is not null)
            {
                currentOperator = operationSymbol;
                FinalizeGroup();
            }
        }

        FinalizeGroup();

        Console.WriteLine("Step two result:");
        Console.WriteLine(total);
    }
}
