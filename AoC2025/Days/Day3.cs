namespace AoC2024;

public class Day3
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(3, false);
        var inputList = _inputFiles.SplitString(input);

        long sumOfLargestPairs = 0;

        foreach (var value in inputList)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var largestPair = FindLargestTwoDigitValue(value.Trim());
            sumOfLargestPairs += largestPair;
        }

        Console.WriteLine($"Step one result: {sumOfLargestPairs}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(3, false);
        Console.WriteLine("Step two not implemented yet.");
    }

    private static int FindLargestTwoDigitValue(string value)
    {
        var largest = 0;

        for (var i = 0; i < value.Length - 1; i++)
        {
            if (!char.IsDigit(value[i]))
            {
                continue;
            }

            var tens = value[i] - '0';

            for (var j = i + 1; j < value.Length; j++)
            {
                if (!char.IsDigit(value[j]))
                {
                    continue;
                }

                var units = value[j] - '0';
                var combined = tens * 10 + units;

                if (combined > largest)
                {
                    largest = combined;
                }
            }
        }

        return largest;
    }
}
