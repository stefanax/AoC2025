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

        var verticalEdges = BuildVerticalEdges(coordinates);
        var horizontalEdges = BuildHorizontalEdges(coordinates);
        var rowSegments = BuildRowSegments(verticalEdges, coordinates);
        var columnSegments = BuildColumnSegments(horizontalEdges, coordinates);

        long maxArea = 0;

        for (var firstIndex = 0; firstIndex < coordinates.Count; firstIndex++)
        {
            var first = coordinates[firstIndex];

            for (var secondIndex = firstIndex + 1; secondIndex < coordinates.Count; secondIndex++)
            {
                var second = coordinates[secondIndex];

                var minX = Math.Min(first.X, second.X);
                var maxX = Math.Max(first.X, second.X);
                var minY = Math.Min(first.Y, second.Y);
                var maxY = Math.Max(first.Y, second.Y);

                if (!RectangleInsideBorder(minX, maxX, minY, maxY, rowSegments, columnSegments))
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

    private static List<(int X, int MinY, int MaxY)> BuildVerticalEdges(List<(int X, int Y)> coordinates)
    {
        var verticalEdges = new List<(int X, int MinY, int MaxY)>();

        for (var index = 0; index < coordinates.Count; index++)
        {
            var current = coordinates[index];
            var next = coordinates[(index + 1) % coordinates.Count];

            if (current.X != next.X)
            {
                continue;
            }

            var minY = Math.Min(current.Y, next.Y);
            var maxY = Math.Max(current.Y, next.Y);
            verticalEdges.Add((current.X, minY, maxY));
        }

        return verticalEdges;
    }

    private static List<(int Y, int MinX, int MaxX)> BuildHorizontalEdges(List<(int X, int Y)> coordinates)
    {
        var horizontalEdges = new List<(int Y, int MinX, int MaxX)>();

        for (var index = 0; index < coordinates.Count; index++)
        {
            var current = coordinates[index];
            var next = coordinates[(index + 1) % coordinates.Count];

            if (current.Y != next.Y)
            {
                continue;
            }

            var minX = Math.Min(current.X, next.X);
            var maxX = Math.Max(current.X, next.X);
            horizontalEdges.Add((current.Y, minX, maxX));
        }

        return horizontalEdges;
    }

    private static List<RowSegment> BuildRowSegments(List<(int X, int MinY, int MaxY)> verticalEdges, List<(int X, int Y)> coordinates)
    {
        var minY = coordinates.Min(c => c.Y);
        var maxY = coordinates.Max(c => c.Y);

        var segments = new List<RowSegment>();
        List<(int Start, int End)>? previousIntervals = null;

        for (var y = minY; y <= maxY; y++)
        {
            var intersections = new List<int>();

            foreach (var edge in verticalEdges)
            {
                if (y >= edge.MinY && y <= edge.MaxY)
                {
                    intersections.Add(edge.X);
                }
            }

            intersections.Sort();

            var intervals = new List<(int Start, int End)>();
            for (var i = 0; i < intersections.Count - 1; i += 2)
            {
                intervals.Add((intersections[i], intersections[i + 1]));
            }

            if (previousIntervals == null || !IntervalsEqual(previousIntervals, intervals))
            {
                if (segments.Count > 0)
                {
                    segments[^1].EndY = y - 1;
                }

                segments.Add(new RowSegment(y, y, intervals));
                previousIntervals = intervals;
            }
        }

        if (segments.Count > 0)
        {
            segments[^1].EndY = maxY;
        }

        return segments;
    }

    private static List<ColumnSegment> BuildColumnSegments(List<(int Y, int MinX, int MaxX)> horizontalEdges, List<(int X, int Y)> coordinates)
    {
        var minX = coordinates.Min(c => c.X);
        var maxX = coordinates.Max(c => c.X);

        var segments = new List<ColumnSegment>();
        List<(int Start, int End)>? previousIntervals = null;

        for (var x = minX; x <= maxX; x++)
        {
            var intersections = new List<int>();

            foreach (var edge in horizontalEdges)
            {
                if (x >= edge.MinX && x <= edge.MaxX)
                {
                    intersections.Add(edge.Y);
                }
            }

            intersections.Sort();

            var intervals = new List<(int Start, int End)>();
            for (var i = 0; i < intersections.Count - 1; i += 2)
            {
                intervals.Add((intersections[i], intersections[i + 1]));
            }

            if (previousIntervals == null || !IntervalsEqual(previousIntervals, intervals))
            {
                if (segments.Count > 0)
                {
                    segments[^1].EndX = x - 1;
                }

                segments.Add(new ColumnSegment(x, x, intervals));
                previousIntervals = intervals;
            }
        }

        if (segments.Count > 0)
        {
            segments[^1].EndX = maxX;
        }

        return segments;
    }

    private static bool IntervalsEqual(List<(int Start, int End)> first, List<(int Start, int End)> second)
    {
        if (first.Count != second.Count)
        {
            return false;
        }

        for (var index = 0; index < first.Count; index++)
        {
            if (first[index] != second[index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool RectangleInsideBorder(int minX, int maxX, int minY, int maxY, List<RowSegment> rowSegments, List<ColumnSegment> columnSegments)
    {
        foreach (var segment in rowSegments)
        {
            if (segment.StartY > maxY)
            {
                break;
            }

            if (segment.EndY < minY)
            {
                continue;
            }

            if (!segment.Intervals.Any(interval => interval.Start <= minX && interval.End >= maxX))
            {
                return false;
            }
        }

        foreach (var segment in columnSegments)
        {
            if (segment.StartX > maxX)
            {
                break;
            }

            if (segment.EndX < minX)
            {
                continue;
            }

            if (!segment.Intervals.Any(interval => interval.Start <= minY && interval.End >= maxY))
            {
                return false;
            }
        }

        return true;
    }

    private class RowSegment
    {
        public RowSegment(int startY, int endY, List<(int Start, int End)> intervals)
        {
            StartY = startY;
            EndY = endY;
            Intervals = intervals;
        }

        public int StartY { get; }

        public int EndY { get; set; }

        public List<(int Start, int End)> Intervals { get; }
    }

    private class ColumnSegment
    {
        public ColumnSegment(int startX, int endX, List<(int Start, int End)> intervals)
        {
            StartX = startX;
            EndX = endX;
            Intervals = intervals;
        }

        public int StartX { get; }

        public int EndX { get; set; }

        public List<(int Start, int End)> Intervals { get; }
    }
}
