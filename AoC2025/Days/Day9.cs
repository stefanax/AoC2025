using System;
using System.Collections.Generic;

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
        _ = _inputFiles.ReadInputFileForDay(9, false);
        Console.WriteLine("Day 9 step 2 not implemented yet.");
    }
}
