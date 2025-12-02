namespace AoC2024;

public class Day2
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(2, false);
        var rangeStrings = input
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        long matchingSum = 0;

        foreach (var range in rangeStrings)
        {
            var rangeParts = range.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (rangeParts.Length != 2
                || !long.TryParse(rangeParts[0], out var start)
                || !long.TryParse(rangeParts[1], out var end))
            {
                continue;
            }

            if (start > end)
            {
                (start, end) = (end, start);
            }

            for (var value = start; value <= end; value++)
            {
                if (IsMatchingPattern(value))
                {
                    matchingSum += value;
                }
            }
        }

        Console.WriteLine($"Step one result: {matchingSum}");
    }

    private static bool IsMatchingPattern(long value)
    {
        if (value < 0)
        {
            return false;
        }

        var valueString = value.ToString();

        if (valueString.Length % 2 != 0)
        {
            return false;
        }

        var halfLength = valueString.Length / 2;
        var firstHalf = valueString[..halfLength];
        var secondHalf = valueString[halfLength..];

        return firstHalf == secondHalf;
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(2, false);
        var rangeStrings = input
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        long matchingSum = 0;

        foreach (var range in rangeStrings)
        {
            var rangeParts = range.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (rangeParts.Length != 2
                || !long.TryParse(rangeParts[0], out var start)
                || !long.TryParse(rangeParts[1], out var end))
            {
                continue;
            }

            if (start > end)
            {
                (start, end) = (end, start);
            }

            for (var value = start; value <= end; value++)
            {
                if (HasRepeatingPattern(value))
                {
                    matchingSum += value;
                }
            }
        }

        Console.WriteLine($"Step two result: {matchingSum}");
    }

    private static bool HasRepeatingPattern(long value)
    {
        if (value < 0)
        {
            return false;
        }

        var valueString = value.ToString();

        if (valueString.Length < 2)
        {
            return false;
        }

        for (var length = 1; length <= valueString.Length / 2; length++)
        {
            if (valueString.Length % length != 0)
            {
                continue;
            }

            var pattern = valueString[..length];
            var matchesPattern = true;

            for (var offset = length; offset < valueString.Length; offset += length)
            {
                if (valueString.Substring(offset, length) != pattern)
                {
                    matchesPattern = false;
                    break;
                }
            }

            if (matchesPattern)
            {
                return true;
            }
        }

        return false;
    }
}
