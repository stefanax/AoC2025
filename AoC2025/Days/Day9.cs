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

        var (minX, maxX, minY, maxY) = BoundingBox(coordinates);
        var rowCoverage = BuildRowCoverage(coordinates, minY, maxY);

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

                if (!RectangleFilled(rectMinX, rectMaxX, rectMinY, rectMaxY, rowCoverage))
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

    private static Dictionary<int, List<(int Start, int End)>> BuildRowCoverage(List<(int X, int Y)> coordinates, int minY, int maxY)
    {
        var intersections = new Dictionary<int, List<int>>();
        var coverage = new Dictionary<int, List<(int Start, int End)>>();

        for (var y = minY; y <= maxY; y++)
        {
            intersections[y] = new List<int>();
            coverage[y] = new List<(int Start, int End)>();
        }

        for (var index = 0; index < coordinates.Count; index++)
        {
            var current = coordinates[index];
            var next = coordinates[(index + 1) % coordinates.Count];

            if (current.X == next.X)
            {
                var yStart = Math.Min(current.Y, next.Y);
                var yEnd = Math.Max(current.Y, next.Y);

                for (var y = yStart; y < yEnd; y++)
                {
                    intersections[y].Add(current.X);
                }
            }
            else if (current.Y == next.Y)
            {
                var xStart = Math.Min(current.X, next.X);
                var xEnd = Math.Max(current.X, next.X);

                AddInterval(coverage[current.Y], xStart, xEnd);
            }
        }

        foreach (var kvp in intersections)
        {
            var y = kvp.Key;
            var rowIntersections = kvp.Value;

            if (rowIntersections.Count < 2)
            {
                continue;
            }

            rowIntersections.Sort();

            for (var i = 0; i + 1 < rowIntersections.Count; i += 2)
            {
                var start = rowIntersections[i];
                var end = rowIntersections[i + 1];
                AddInterval(coverage[y], start, end);
            }
        }

        return coverage;
    }

    private static void AddInterval(List<(int Start, int End)> intervals, int start, int end)
    {
        if (start > end)
        {
            (start, end) = (end, start);
        }

        var index = 0;

        while (index < intervals.Count && intervals[index].End < start - 1)
        {
            index++;
        }

        while (index < intervals.Count && intervals[index].Start <= end + 1)
        {
            start = Math.Min(start, intervals[index].Start);
            end = Math.Max(end, intervals[index].End);
            intervals.RemoveAt(index);
        }

        intervals.Insert(index, (start, end));
    }

    private static bool RectangleFilled(int minX, int maxX, int minY, int maxY, Dictionary<int, List<(int Start, int End)>> rowCoverage)
    {
        for (var y = minY; y <= maxY; y++)
        {
            if (!rowCoverage.TryGetValue(y, out var intervals) || intervals.Count == 0)
            {
                return false;
            }

            var covered = intervals.Any(interval => interval.Start <= minX && interval.End >= maxX);

            if (!covered)
            {
                return false;
            }
        }

        return true;
    }
}
