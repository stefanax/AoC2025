using System;
using System.Collections.Generic;

namespace AoC2024;

public class Day8
{
    private readonly InputFiles _inputFiles = new InputFiles();

    private sealed class UnionFind
    {
        private readonly int[] _parents;
        private readonly int[] _sizes;

        public UnionFind(int count)
        {
            _parents = new int[count];
            _sizes = new int[count];

            for (var index = 0; index < count; index++)
            {
                _parents[index] = index;
                _sizes[index] = 1;
            }
        }

        private int Find(int index)
        {
            if (_parents[index] == index)
            {
                return index;
            }

            _parents[index] = Find(_parents[index]);
            return _parents[index];
        }

        public void Union(int first, int second)
        {
            var rootFirst = Find(first);
            var rootSecond = Find(second);

            if (rootFirst == rootSecond)
            {
                return;
            }

            if (_sizes[rootFirst] < _sizes[rootSecond])
            {
                (rootFirst, rootSecond) = (rootSecond, rootFirst);
            }

            _parents[rootSecond] = rootFirst;
            _sizes[rootFirst] += _sizes[rootSecond];
        }

        public int FindRoot(int index)
        {
            return Find(index);
        }

        public int GroupSize(int index)
        {
            var root = Find(index);
            return _sizes[root];
        }
    }

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(8, false);
        var inputList = _inputFiles.SplitString(input);
        var coordinates = new List<(int X, int Y, int Z)>();

        foreach (var line in inputList)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            coordinates.Add((int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));
        }

        var pairDistances = new List<(long DistanceSquared, int FirstIndex, int SecondIndex)>();

        for (var firstIndex = 0; firstIndex < coordinates.Count; firstIndex++)
        {
            for (var secondIndex = firstIndex + 1; secondIndex < coordinates.Count; secondIndex++)
            {
                var first = coordinates[firstIndex];
                var second = coordinates[secondIndex];

                var deltaX = first.X - second.X;
                var deltaY = first.Y - second.Y;
                var deltaZ = first.Z - second.Z;

                var distanceSquared = (long)deltaX * deltaX + (long)deltaY * deltaY + (long)deltaZ * deltaZ;
                pairDistances.Add((distanceSquared, firstIndex, secondIndex));
            }
        }

        pairDistances.Sort((left, right) => left.DistanceSquared.CompareTo(right.DistanceSquared));

        var unionFind = new UnionFind(coordinates.Count);
        var iterations = Math.Min(1000, pairDistances.Count);

        for (var index = 0; index < iterations; index++)
        {
            var (_, firstIndex, secondIndex) = pairDistances[index];
            unionFind.Union(firstIndex, secondIndex);
        }

        var groups = new Dictionary<int, int>();

        for (var index = 0; index < coordinates.Count; index++)
        {
            var root = unionFind.FindRoot(index);

            if (!groups.ContainsKey(root))
            {
                groups[root] = unionFind.GroupSize(index);
            }
        }

        var topGroupSizes = new List<int>(groups.Values);
        topGroupSizes.Sort((left, right) => right.CompareTo(left));

        var groupCount = Math.Min(3, topGroupSizes.Count);
        var result = 1L;

        for (var index = 0; index < groupCount; index++)
        {
            result *= topGroupSizes[index];
        }

        Console.WriteLine("Step one result:");
        Console.WriteLine(result);
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(8, false);
        var inputList = _inputFiles.SplitString(input);
        _ = inputList;

        Console.WriteLine("Step two result:");
        Console.WriteLine("Not implemented yet.");
    }
}
