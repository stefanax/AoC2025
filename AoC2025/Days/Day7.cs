using System.Collections.Generic;
using System.Numerics;

namespace AoC2024;

public class Day7
{
    private readonly InputFiles _inputFiles = new InputFiles();

    private (int Row, int Column) FindStartPosition(string[] grid)
    {
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

        return (startRow, startColumn);
    }

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(7, false);
        var grid = _inputFiles.SplitString(input);

        var (startRow, startColumn) = FindStartPosition(grid);

        var currentPositions = new HashSet<int> { startColumn };
        var splitCount = 0;

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
                    splitCount++;
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
        Console.WriteLine(splitCount);
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(7, false);
        var grid = _inputFiles.SplitString(input);

        var (startRow, startColumn) = FindStartPosition(grid);

        var currentPaths = new Dictionary<int, BigInteger> { { startColumn, BigInteger.One } };

        for (var rowIndex = startRow + 1; rowIndex < grid.Length; rowIndex++)
        {
            var row = grid[rowIndex];
            var nextPaths = new Dictionary<int, BigInteger>();

            void AddPath(int column, BigInteger count)
            {
                if (column < 0 || column >= row.Length)
                {
                    return;
                }

                if (nextPaths.TryGetValue(column, out var existing))
                {
                    nextPaths[column] = existing + count;
                }
                else
                {
                    nextPaths[column] = count;
                }
            }

            foreach (var (column, count) in currentPaths)
            {
                if (column < 0 || column >= row.Length)
                {
                    continue;
                }

                var cell = row[column];

                if (cell == '^')
                {
                    AddPath(column - 1, count);
                    AddPath(column + 1, count);
                }
                else
                {
                    AddPath(column, count);
                }
            }

            currentPaths = nextPaths;
        }

        var totalPaths = BigInteger.Zero;

        foreach (var count in currentPaths.Values)
        {
            totalPaths += count;
        }

        Console.WriteLine("Step two result:");
        Console.WriteLine(totalPaths);
    }
}
