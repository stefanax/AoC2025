using System;
using System.Collections.Generic;
using System.Linq;

namespace AoC2024;

public class Day9
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(9, false);
        var inputList = _inputFiles.SplitString(input);
        var coordinates = new List<(int X, int Y)>();

        foreach (var line in inputList)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            coordinates.Add((int.Parse(parts[0]), int.Parse(parts[1])));
        }

        var maxArea = 0L;

        for (var firstIndex = 0; firstIndex < coordinates.Count; firstIndex++)
        {
            var first = coordinates[firstIndex];

            for (var secondIndex = firstIndex + 1; secondIndex < coordinates.Count; secondIndex++)
            {
                var second = coordinates[secondIndex];

                var width = Math.Abs((long)(first.X - second.X)) + 1;
                var height = Math.Abs((long)(first.Y - second.Y)) + 1;

                var area = width * height;
                maxArea = Math.Max(maxArea, area);
            }
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(maxArea);
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(9, false);
        var inputList = _inputFiles.SplitString(input);
        var coordinates = new List<(int X, int Y)>();

        foreach (var line in inputList)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            coordinates.Add((int.Parse(parts[0]), int.Parse(parts[1])));
        }

        var redTiles = new HashSet<(int X, int Y)>(coordinates);
        var (minX, maxX, minY, maxY) = BoundingBox(coordinates);
        var grid = BuildTileGrid(coordinates, redTiles, minX, maxX, minY, maxY);

        long maxArea = 0;

        for (var firstIndex = 0; firstIndex < coordinates.Count; firstIndex++)
        {
            var first = coordinates[firstIndex];

            for (var secondIndex = firstIndex + 1; secondIndex < coordinates.Count; secondIndex++)
            {
                var second = coordinates[secondIndex];

                var rectMinX = Math.Min(first.X, second.X);
                var rectMaxX = Math.Max(first.X, second.X);
                var rectMinY = Math.Min(first.Y, second.Y);
                var rectMaxY = Math.Max(first.Y, second.Y);

                if (!RectangleFilled(rectMinX, rectMaxX, rectMinY, rectMaxY, grid, minX, minY))
                {
                    continue;
                }

                var width = Math.Abs((long)(first.X - second.X)) + 1;
                var height = Math.Abs((long)(first.Y - second.Y)) + 1;
                var area = width * height;
                maxArea = Math.Max(maxArea, area);
            }
        }

        Console.WriteLine("Step two result:");
        Console.WriteLine(maxArea);
    }

    private static (int MinX, int MaxX, int MinY, int MaxY) BoundingBox(List<(int X, int Y)> coordinates)
    {
        var minX = coordinates.Min(c => c.X);
        var maxX = coordinates.Max(c => c.X);
        var minY = coordinates.Min(c => c.Y);
        var maxY = coordinates.Max(c => c.Y);

        return (minX, maxX, minY, maxY);
    }

    private static TileState[,] BuildTileGrid(List<(int X, int Y)> coordinates, HashSet<(int X, int Y)> redTiles, int minX, int maxX, int minY, int maxY)
    {
        var width = maxX - minX + 3;
        var height = maxY - minY + 3;
        var grid = new TileState[width, height];

        foreach (var red in redTiles)
        {
            grid[red.X - minX + 1, red.Y - minY + 1] = TileState.Red;
        }

        for (var index = 0; index < coordinates.Count; index++)
        {
            var current = coordinates[index];
            var next = coordinates[(index + 1) % coordinates.Count];

            if (current.X == next.X)
            {
                var start = Math.Min(current.Y, next.Y);
                var end = Math.Max(current.Y, next.Y);

                for (var y = start; y <= end; y++)
                {
                    var point = (current.X, y);
                    if (!redTiles.Contains(point))
                    {
                        grid[current.X - minX + 1, y - minY + 1] = TileState.Green;
                    }
                }
            }
            else if (current.Y == next.Y)
            {
                var start = Math.Min(current.X, next.X);
                var end = Math.Max(current.X, next.X);

                for (var x = start; x <= end; x++)
                {
                    var point = (x, current.Y);
                    if (!redTiles.Contains(point))
                    {
                        grid[x - minX + 1, current.Y - minY + 1] = TileState.Green;
                    }
                }
            }
        }

        FloodFillOutside(grid, minX, minY);

        for (var x = 1; x < width - 1; x++)
        {
            for (var y = 1; y < height - 1; y++)
            {
                if (grid[x, y] == TileState.Unknown)
                {
                    grid[x, y] = TileState.Green;
                }
            }
        }

        return grid;
    }

    private static void FloodFillOutside(TileState[,] grid, int minX, int minY)
    {
        var width = grid.GetLength(0);
        var height = grid.GetLength(1);
        var queue = new Queue<(int X, int Y)>();
        var visited = new bool[width, height];

        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        var directions = new (int X, int Y)[]
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1)
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var direction in directions)
            {
                var nextX = current.X + direction.X;
                var nextY = current.Y + direction.Y;

                if (nextX < 0 || nextX >= width || nextY < 0 || nextY >= height)
                {
                    continue;
                }

                if (visited[nextX, nextY])
                {
                    continue;
                }

                if (grid[nextX, nextY] != TileState.Unknown)
                {
                    continue;
                }

                visited[nextX, nextY] = true;
                queue.Enqueue((nextX, nextY));
            }
        }

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (visited[x, y])
                {
                    grid[x, y] = TileState.Outside;
                }
            }
        }
    }

    private static bool RectangleFilled(int minX, int maxX, int minY, int maxY, TileState[,] grid, int originX, int originY)
    {
        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                var state = grid[x - originX + 1, y - originY + 1];
                if (state != TileState.Green && state != TileState.Red)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private enum TileState
    {
        Unknown,
        Red,
        Green,
        Outside
    }
}
