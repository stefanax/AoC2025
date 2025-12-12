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
        var input = _inputFiles.ReadInputFileForDay(10, false);
        var lines = _inputFiles.SplitString(input);
        var machines = ParseMachinesForStep2(lines);

        var totalPresses = 0L;

        foreach (var machine in machines)
        {
            var presses = MinimumPressesToReachLevels(machine);

            totalPresses += presses;
        }

        Console.WriteLine("Step two result:");
        Console.WriteLine(totalPresses);
    }

    private static List<(BigInteger[] Target, List<List<int>> Buttons)> ParseMachinesForStep2(string[] lines)
    {
        var machines = new List<(BigInteger[] Target, List<List<int>> Buttons)>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            machines.Add((ParseJoltageTargets(line), ParseButtons(line)));
        }

        return machines;
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

    private static BigInteger[] ParseJoltageTargets(string line)
    {
        var lastBraceIndex = line.LastIndexOf('{');

        if (lastBraceIndex == -1)
        {
            throw new InvalidOperationException("Machine line missing joltage targets.");
        }

        var closingIndex = line.IndexOf('}', lastBraceIndex + 1);

        if (closingIndex == -1)
        {
            throw new InvalidOperationException("Machine line missing joltage targets.");
        }

        var content = line.Substring(lastBraceIndex + 1, closingIndex - lastBraceIndex - 1);
        var parts = content.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
        {
            throw new InvalidOperationException("Machine line missing joltage targets.");
        }

        return parts.Select(BigInteger.Parse).ToArray();
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

    private static long MinimumPressesToReachLevels((BigInteger[] Target, List<List<int>> Buttons) machine)
    {
        var target = machine.Target;
        var buttons = machine.Buttons;

        var numberOfPositions = target.Length;
        var numberOfButtons = buttons.Count;

        if (numberOfButtons == 0)
        {
            if (target.Any(t => t != 0))
            {
                throw new InvalidOperationException("Machine configuration has no solution.");
            }

            return 0;
        }

        var matrix = new BigInteger[numberOfPositions, numberOfButtons];
        var rhs = new BigInteger[numberOfPositions];

        for (var position = 0; position < numberOfPositions; position++)
        {
            rhs[position] = target[position];
        }

        for (var buttonIndex = 0; buttonIndex < numberOfButtons; buttonIndex++)
        {
            foreach (var positionIndex in buttons[buttonIndex])
            {
                if (positionIndex >= 0 && positionIndex < numberOfPositions)
                {
                    matrix[positionIndex, buttonIndex] = 1;
                }
            }
        }

        var (solutionExists, particularSolution, nullspace) = SolveIntegerSystem(matrix, rhs);

        if (!solutionExists)
        {
            throw new InvalidOperationException("Machine configuration has no solution.");
        }

        var minPresses = MinimizeIntegerPresses(particularSolution, nullspace);

        return (long)minPresses;
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

    private static (bool SolutionExists, BigInteger[] ParticularSolution, List<BigInteger[]> NullspaceBasis) SolveIntegerSystem(BigInteger[,] matrix, BigInteger[] rhs)
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
                if (matrix[row, col] != 0)
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
                if (row == currentRow || matrix[row, col] == 0)
                {
                    continue;
                }

                var factor = matrix[row, col];

                for (var c = col; c < cols; c++)
                {
                    matrix[row, c] -= factor * matrix[currentRow, c];
                }

                rhs[row] -= factor * rhs[currentRow];
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
                if (matrix[row, col] != 0)
                {
                    hasCoefficient = true;
                    break;
                }
            }

            if (!hasCoefficient && rhs[row] != 0)
            {
                return (false, Array.Empty<BigInteger>(), new List<BigInteger[]>());
            }
        }

        var particularSolution = new BigInteger[cols];

        for (var index = pivotColumns.Count - 1; index >= 0; index--)
        {
            var col = pivotColumns[index];
            var row = pivotRows[index];
            var value = rhs[row];

            for (var c = col + 1; c < cols; c++)
            {
                if (matrix[row, c] != 0)
                {
                    value -= matrix[row, c] * particularSolution[c];
                }
            }

            particularSolution[col] = value;
        }

        var pivotSet = pivotColumns.ToHashSet();
        var nullspace = new List<BigInteger[]>();

        for (var freeCol = 0; freeCol < cols; freeCol++)
        {
            if (pivotSet.Contains(freeCol))
            {
                continue;
            }

            var vector = new BigInteger[cols];
            vector[freeCol] = 1;

            for (var index = pivotColumns.Count - 1; index >= 0; index--)
            {
                var col = pivotColumns[index];
                var row = pivotRows[index];
                var value = BigInteger.Zero;

                for (var c = col + 1; c < cols; c++)
                {
                    if (matrix[row, c] != 0)
                    {
                        value -= matrix[row, c] * vector[c];
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

    private static BigInteger MinimizeIntegerPresses(BigInteger[] particularSolution, List<BigInteger[]> nullspaceBasis)
    {
        var current = (BigInteger[])particularSolution.Clone();

        if (nullspaceBasis.Count == 0)
        {
            if (current.Any(v => v < 0))
            {
                throw new InvalidOperationException("Machine configuration has no non-negative solution.");
            }

            return current.Aggregate(BigInteger.Zero, static (acc, val) => acc + val);
        }

        var basisContributions = nullspaceBasis
            .Select(v => v.Aggregate(BigInteger.Zero, static (acc, val) => acc + val))
            .ToArray();

        BigInteger? best = null;

        void Search(int index)
        {
            if (index == nullspaceBasis.Count)
            {
                if (current.All(v => v >= 0))
                {
                    var presses = current.Aggregate(BigInteger.Zero, static (acc, val) => acc + val);

                    if (best is null || presses < best)
                    {
                        best = presses;
                    }
                }

                return;
            }

            var basis = nullspaceBasis[index];

            var min = BigInteger.Zero;
            var max = BigInteger.Zero;
            var hasConstraint = false;

            for (var i = 0; i < current.Length; i++)
            {
                var coefficient = basis[i];

                if (coefficient == 0)
                {
                    continue;
                }

                var value = current[i];

                if (coefficient > 0)
                {
                    var bound = DivCeiling(-value, coefficient);
                    min = hasConstraint ? BigInteger.Max(min, bound) : bound;
                }
                else
                {
                    var bound = value / (-coefficient);
                    max = hasConstraint ? BigInteger.Min(max, bound) : bound;
                }

                hasConstraint = true;
            }

            if (!hasConstraint)
            {
                Search(index + 1);
                return;
            }

            if (min > max)
            {
                return;
            }

            var contribution = basisContributions[index];

            IEnumerable<BigInteger> Range()
            {
                if (contribution > 0)
                {
                    for (var t = min; t <= max; t++)
                    {
                        yield return t;
                    }
                }
                else
                {
                    for (var t = max; t >= min; t--)
                    {
                        yield return t;
                    }
                }
            }

            foreach (var t in Range())
            {
                AddScaled(current, basis, t);

                var presses = current.Aggregate(BigInteger.Zero, static (acc, val) => acc + val);

                if (best is null || presses < best)
                {
                    Search(index + 1);
                }

                AddScaled(current, basis, -t);
            }
        }

        Search(0);

        if (best is null)
        {
            throw new InvalidOperationException("Machine configuration has no non-negative solution.");
        }

        return best.Value;
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

    private static void AddScaled(BigInteger[] target, IReadOnlyList<BigInteger> source, BigInteger scale)
    {
        for (var i = 0; i < target.Length; i++)
        {
            target[i] += source[i] * scale;
        }
    }

    private static BigInteger DivCeiling(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0)
        {
            throw new DivideByZeroException();
        }

        if (numerator >= 0)
        {
            return (numerator + denominator - 1) / denominator;
        }

        return numerator / denominator;
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

    private static void SwapRows(BigInteger[,] matrix, BigInteger[] rhs, int rowA, int rowB)
    {
        var columns = matrix.GetLength(1);

        for (var col = 0; col < columns; col++)
        {
            (matrix[rowA, col], matrix[rowB, col]) = (matrix[rowB, col], matrix[rowA, col]);
        }

        (rhs[rowA], rhs[rowB]) = (rhs[rowB], rhs[rowA]);
    }
}
