namespace AoC2024;

public class Day1
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(1, false);
        var inputList = _inputFiles.SplitString(input);

        var result = "temp value";
        
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