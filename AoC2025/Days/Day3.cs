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

            var largestPair = FindLargestNumber(value.Trim(), 2);
            sumOfLargestPairs += largestPair;
        }

        Console.WriteLine($"Step one result: {sumOfLargestPairs}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(3, false);
        var inputList = _inputFiles.SplitString(input);

        long sumOfLargestTwelveDigitNumbers = 0;

        foreach (var value in inputList)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var largestTwelveDigitNumber = FindLargestNumber(value.Trim(), 12);
            sumOfLargestTwelveDigitNumbers += largestTwelveDigitNumber;
        }

        Console.WriteLine($"Step two result: {sumOfLargestTwelveDigitNumbers}");
    }

    private static long FindLargestNumber(string value, int length)
    {
        var digits = new List<int>();

        foreach (var ch in value)
        {
            if (char.IsDigit(ch))
            {
                digits.Add(ch - '0');
            }
        }

        if (digits.Count < length)
        {
            return 0;
        }

        var stack = new List<int>();

        for (var i = 0; i < digits.Count; i++)
        {
            var digit = digits[i];
            var remainingDigits = digits.Count - i - 1;

            while (stack.Count > 0 && stack[^1] < digit && stack.Count + remainingDigits >= length)
            {
                stack.RemoveAt(stack.Count - 1);
            }

            if (stack.Count < length)
            {
                stack.Add(digit);
            }
        }

        long result = 0;

        foreach (var digit in stack)
        {
            result = result * 10 + digit;
        }

        return result;
    }
}
