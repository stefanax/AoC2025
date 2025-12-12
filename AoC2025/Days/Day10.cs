using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AoC2024;

public class Day10
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(10, false);
        var lines = _inputFiles.SplitString(input);
        var machines = ParseMachines(lines);

        var totalPresses = 0L;

        foreach (var machine in machines)
        {
            var presses = MinimumPresses(machine);

            if (presses == int.MaxValue)
            {
                throw new InvalidOperationException("Machine configuration has no solution.");
            }

            totalPresses += presses;
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(totalPresses);
    }

    public void Step2()
    {
        _ = _inputFiles.ReadInputFileForDay(10, false);
        Console.WriteLine("Day 10 step 2 not implemented yet.");
    }

    private static List<(bool[] Target, List<List<int>> Buttons)> ParseMachines(string[] lines)
    {
        var machines = new List<(bool[] Target, List<List<int>> Buttons)>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            machines.Add(ParseMachineLine(line));
        }

        return machines;
    }

    private static (bool[] Target, List<List<int>> Buttons) ParseMachineLine(string line)
    {
        var target = ParseTarget(line);
        var buttons = ParseButtons(line);

        return (target, buttons);
    }

    private static bool[] ParseTarget(string line)
    {
        var firstTargetIndex = line.IndexOfAny(['#', '.']);

        if (firstTargetIndex == -1)
        {
            throw new InvalidOperationException("Machine line missing target state.");
        }

        var targetChars = line[firstTargetIndex..].TakeWhile(c => c is '#' or '.').ToArray();

        if (targetChars.Length == 0)
        {
            throw new InvalidOperationException("Machine line missing target state.");
        }

        return targetChars.Select(c => c == '#').ToArray();
    }

    private static List<List<int>> ParseButtons(string line)
    {
        var buttons = new List<List<int>>();
        var startIndex = 0;

        while (true)
        {
            var openIndex = line.IndexOf('(', startIndex);

            if (openIndex == -1)
            {
                break;
            }

            var closeIndex = line.IndexOf(')', openIndex + 1);

            if (closeIndex == -1)
            {
                break;
            }

            var content = line.Substring(openIndex + 1, closeIndex - openIndex - 1);

            if (!string.IsNullOrWhiteSpace(content))
            {
                var numbers = content.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => int.Parse(s.Trim()))
                    .ToList();

                buttons.Add(numbers);
            }

            startIndex = closeIndex + 1;
        }

        return buttons;
    }

    private static int MinimumPresses((bool[] Target, List<List<int>> Buttons) machine)
    {
        var target = machine.Target;
        var buttons = machine.Buttons;

        var numberOfLights = target.Length;
        var numberOfButtons = buttons.Count;

        if (numberOfButtons == 0)
        {
            return target.Any(t => t) ? int.MaxValue : 0;
        }

        var matrix = new bool[numberOfLights, numberOfButtons];
        var rhs = new bool[numberOfLights];

        for (var light = 0; light < numberOfLights; light++)
        {
            rhs[light] = target[light];
        }

        for (var buttonIndex = 0; buttonIndex < numberOfButtons; buttonIndex++)
        {
            foreach (var lightIndex in buttons[buttonIndex])
            {
                if (lightIndex >= 0 && lightIndex < numberOfLights)
                {
                    matrix[lightIndex, buttonIndex] = true;
                }
            }
        }

        var (solutionExists, particularSolution, nullspace) = SolveLinearSystem(matrix, rhs);

        if (!solutionExists)
        {
            return int.MaxValue;
        }

        return MinimizePresses(particularSolution, nullspace);
    }

    private static (bool SolutionExists, bool[] ParticularSolution, List<bool[]> NullspaceBasis) SolveLinearSystem(bool[,] matrix, bool[] rhs)
    {
        var rows = matrix.GetLength(0);
        var cols = matrix.GetLength(1);

        var pivotColumns = new List<int>();
        var pivotRows = new List<int>();

        var currentRow = 0;

        for (var col = 0; col < cols && currentRow < rows; col++)
        {
            var pivotRow = -1;

            for (var row = currentRow; row < rows; row++)
            {
                if (matrix[row, col])
                {
                    pivotRow = row;
                    break;
                }
            }

            if (pivotRow == -1)
            {
                continue;
            }

            if (pivotRow != currentRow)
            {
                SwapRows(matrix, rhs, currentRow, pivotRow);
            }

            for (var row = 0; row < rows; row++)
            {
                if (row != currentRow && matrix[row, col])
                {
                    for (var c = col; c < cols; c++)
                    {
                        matrix[row, c] ^= matrix[currentRow, c];
                    }

                    rhs[row] ^= rhs[currentRow];
                }
            }

            pivotColumns.Add(col);
            pivotRows.Add(currentRow);
            currentRow++;
        }

        for (var row = currentRow; row < rows; row++)
        {
            var hasCoefficient = false;

            for (var col = 0; col < cols; col++)
            {
                if (matrix[row, col])
                {
                    hasCoefficient = true;
                    break;
                }
            }

            if (!hasCoefficient && rhs[row])
            {
                return (false, Array.Empty<bool>(), new List<bool[]>());
            }
        }

        var particularSolution = new bool[cols];

        for (var index = pivotColumns.Count - 1; index >= 0; index--)
        {
            var col = pivotColumns[index];
            var row = pivotRows[index];
            var value = rhs[row];

            for (var c = col + 1; c < cols; c++)
            {
                if (matrix[row, c] && particularSolution[c])
                {
                    value ^= true;
                }
            }

            particularSolution[col] = value;
        }

        var pivotSet = pivotColumns.ToHashSet();
        var nullspace = new List<bool[]>();

        for (var freeCol = 0; freeCol < cols; freeCol++)
        {
            if (pivotSet.Contains(freeCol))
            {
                continue;
            }

            var vector = new bool[cols];
            vector[freeCol] = true;

            for (var index = pivotColumns.Count - 1; index >= 0; index--)
            {
                var col = pivotColumns[index];
                var row = pivotRows[index];
                var value = false;

                for (var c = col + 1; c < cols; c++)
                {
                    if (matrix[row, c] && vector[c])
                    {
                        value ^= true;
                    }
                }

                vector[col] = value;
            }

            nullspace.Add(vector);
        }

        return (true, particularSolution, nullspace);
    }

    private static int MinimizePresses(bool[] particularSolution, List<bool[]> nullspaceBasis)
    {
        var buttonCount = particularSolution.Length;
        var segments = (buttonCount + 63) / 64;

        var particularSegments = ToSegments(particularSolution, segments);
        var basisSegments = nullspaceBasis.Select(v => ToSegments(v, segments)).ToList();

        var basisCount = basisSegments.Count;

        var minPresses = PopCount(particularSegments);

        if (basisCount == 0)
        {
            return minPresses;
        }

        if (basisCount >= 63)
        {
            throw new InvalidOperationException("Too many free variables to enumerate solutions.");
        }

        var totalCombinations = 1L << basisCount;
        var current = (ulong[])particularSegments.Clone();

        var previousGray = 0L;

        for (long mask = 0; mask < totalCombinations; mask++)
        {
            if (mask != 0)
            {
                var gray = mask ^ (mask >> 1);
                var change = previousGray ^ gray;
                var basisIndex = BitOperations.TrailingZeroCount((ulong)change);

                XorSegments(current, basisSegments[basisIndex]);
                previousGray = gray;
            }

            var weight = PopCount(current);
            minPresses = Math.Min(minPresses, weight);
        }

        return minPresses;
    }

    private static ulong[] ToSegments(IReadOnlyList<bool> vector, int segments)
    {
        var result = new ulong[segments];

        for (var i = 0; i < vector.Count; i++)
        {
            if (!vector[i])
            {
                continue;
            }

            var segmentIndex = i / 64;
            var bitIndex = i % 64;
            result[segmentIndex] |= 1UL << bitIndex;
        }

        return result;
    }

    private static void XorSegments(ulong[] target, ulong[] source)
    {
        for (var i = 0; i < target.Length; i++)
        {
            target[i] ^= source[i];
        }
    }

    private static int PopCount(IReadOnlyList<ulong> segments)
    {
        var count = 0;

        for (var i = 0; i < segments.Count; i++)
        {
            count += BitOperations.PopCount(segments[i]);
        }

        return count;
    }

    private static void SwapRows(bool[,] matrix, bool[] rhs, int rowA, int rowB)
    {
        var columns = matrix.GetLength(1);

        for (var col = 0; col < columns; col++)
        {
            (matrix[rowA, col], matrix[rowB, col]) = (matrix[rowB, col], matrix[rowA, col]);
        }

        (rhs[rowA], rhs[rowB]) = (rhs[rowB], rhs[rowA]);
    }
}
