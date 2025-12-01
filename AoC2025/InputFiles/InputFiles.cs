namespace AoC2024;

public class InputFiles
{
    public string ReadInputFileForDay(int day, bool testInput = false)
    {
        var testInputString = testInput ? "_test" : "";
        var input = File.ReadAllText($"../../../InputFiles\\day{day}{testInputString}.txt");

        return input;
    }

    public string[] SplitString(string input)
    {
        return input.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
    }
}