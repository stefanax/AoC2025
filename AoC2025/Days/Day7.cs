using System.Collections.Generic;
using System.Linq;

namespace AoC2024;

public class Day7
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(7, false);

        var rows = _inputFiles.SplitString(input)
            .Select(line => line.TrimEnd('\r'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();

        if (rows.Count == 0)
        {
            throw new InvalidOperationException("Input must contain at least one row.");
        }

        var width = rows.Max(line => line.Length);
        var normalizedRows = rows
            .Select(line => line.PadRight(width, '.'))
            .ToList();

        var startRow = -1;
        var startColumn = -1;

        for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            var columnIndex = rows[rowIndex].IndexOf('S');

            if (columnIndex == -1)
            {
                continue;
            }

            if (startRow != -1)
            {
                throw new InvalidOperationException("Input must contain exactly one starting position 'S'.");
            }

            startRow = rowIndex;
            startColumn = columnIndex;
        }

        if (startRow == -1)
        {
            throw new InvalidOperationException("Input must contain a starting position 'S'.");
        }

        var currentColumns = new HashSet<int> { startColumn };

        for (var rowIndex = startRow + 1; rowIndex < rows.Count; rowIndex++)
        {
            var nextColumns = new HashSet<int>();

            foreach (var column in currentColumns)
            {
                var cell = column >= 0 && column < rows[rowIndex].Length
                    ? rows[rowIndex][column]
                    : '.';

                if (cell == '^')
                {
                    if (column - 1 >= 0)
                    {
                        nextColumns.Add(column - 1);
                    }

                    if (column + 1 < width)
                    {
                        nextColumns.Add(column + 1);
                    }

                    continue;
                }

                nextColumns.Add(column);
            }

            currentColumns = nextColumns;
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(currentColumns.Count);

    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(7, false);

        Console.WriteLine("Step two result:");
        Console.WriteLine("Not implemented yet.");
    }
}
