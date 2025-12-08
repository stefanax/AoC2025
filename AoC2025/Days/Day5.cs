namespace AoC2024;

public class Day5
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(5, false);
        var inputList = _inputFiles.SplitString(input);

        var ranges = new List<(long Start, long End)>();

        var index = 0;

        while (index < inputList.Length && !string.IsNullOrWhiteSpace(inputList[index]))
        {
            var rangeParts = inputList[index].Split('-');
            var start = long.Parse(rangeParts[0]);
            var end = long.Parse(rangeParts[1]);

            ranges.Add((start, end));
            index++;
        }

        if (index < inputList.Length && string.IsNullOrWhiteSpace(inputList[index]))
        {
            index++;
        }

        var goodValueCount = 0;

        while (index < inputList.Length)
        {
            var line = inputList[index];

            if (!string.IsNullOrWhiteSpace(line))
            {
                var value = long.Parse(line);

                if (ranges.Any(range => value >= range.Start && value <= range.End))
                {
                    goodValueCount++;
                }
            }

            index++;
        }

        Console.WriteLine($"Step one result: {goodValueCount}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(5, false);
        var inputList = _inputFiles.SplitString(input);

        var ranges = new List<(long Start, long End)>();

        var index = 0;

        while (index < inputList.Length && !string.IsNullOrWhiteSpace(inputList[index]))
        {
            var rangeParts = inputList[index].Split('-');
            var start = long.Parse(rangeParts[0]);
            var end = long.Parse(rangeParts[1]);

            ranges.Add((start, end));
            index++;
        }

        var mergedRanges = new List<(long Start, long End)>();

        foreach (var range in ranges.OrderBy(r => r.Start))
        {
            if (mergedRanges.Count == 0)
            {
                mergedRanges.Add(range);
                continue;
            }

            var lastRange = mergedRanges[^1];

            if (range.Start <= lastRange.End + 1)
            {
                mergedRanges[^1] = (lastRange.Start, Math.Max(lastRange.End, range.End));
            }
            else
            {
                mergedRanges.Add(range);
            }
        }
        var totalValues = mergedRanges.Sum(range => range.End - range.Start + 1);

        Console.WriteLine($"Step two result: {totalValues}");
    }
}
