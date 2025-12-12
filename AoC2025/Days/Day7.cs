using System.Collections.Generic;

namespace AoC2024;

public class Day7
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(7, false);
        var grid = _inputFiles.SplitString(input);

        var startRow = -1;
        var startColumn = -1;

        for (var rowIndex = 0; rowIndex < grid.Length; rowIndex++)
        {
            var columnIndex = grid[rowIndex].IndexOf('S');

            if (columnIndex == -1)
            {
                continue;
            }

            startRow = rowIndex;
            startColumn = columnIndex;
            break;
        }

        if (startRow == -1 || startColumn == -1)
        {
            throw new InvalidOperationException("Input must contain a starting position 'S'.");
        }

        var currentPositions = new HashSet<int> { startColumn };

        for (var rowIndex = startRow + 1; rowIndex < grid.Length; rowIndex++)
        {
            var row = grid[rowIndex];
            var nextPositions = new HashSet<int>();

            void AddPositionIfValid(int column)
            {
                if (column >= 0 && column < row.Length)
                {
                    nextPositions.Add(column);
                }
            }

            foreach (var position in currentPositions)
            {
                if (position < 0 || position >= row.Length)
                {
                    continue;
                }

                var cell = row[position];

                if (cell == '^')
                {
                    AddPositionIfValid(position - 1);
                    AddPositionIfValid(position + 1);
                }
                else
                {
                    AddPositionIfValid(position);
                }
            }

            currentPositions = nextPositions;
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(currentPositions.Count);
    }

    public void Step2()
    {
        _ = _inputFiles.ReadInputFileForDay(7, false);

        Console.WriteLine("Step two result:");
        Console.WriteLine("Not implemented yet.");
    }
}
