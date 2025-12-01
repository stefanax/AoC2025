namespace AoC2024;

public class Day1
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(1, false);
        var inputList = _inputFiles.SplitString(input);

        const int dialSize = 100;
        var currentPosition = 50;
        var zeroHits = 0;

        foreach (var instruction in inputList)
        {
            if (string.IsNullOrWhiteSpace(instruction))
            {
                continue;
            }

            var direction = instruction[0];
            var steps = int.Parse(instruction[1..]);

            for (var i = 0; i < steps; i++)
            {
                if (direction == 'L')
                {
                    currentPosition = (currentPosition - 1 + dialSize) % dialSize;
                }
                else if (direction == 'R')
                {
                    currentPosition = (currentPosition + 1) % dialSize;
                }

                if (currentPosition == 0)
                {
                    zeroHits++;
                }
            }
        }

        var result = zeroHits;

        Console.WriteLine($"Step one result: {result}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(1, false);
        var inputList = _inputFiles.SplitString(input);
        
        var result = "temp value";
        
        Console.WriteLine($"Step two result: {result}");
    }
}