namespace AoC2024;

public class Day1
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(1, false);
        var inputList = _inputFiles.SplitString(input);
        
        var listOne = new List<int>();
        var listTwo = new List<int>();

        foreach (var inputString in inputList)
        {
            var inputValues = inputString.Split("   ");
            listOne.Add(int.Parse(inputValues[0]));
            listTwo.Add(int.Parse(inputValues[1]));
        }
        
        listOne.Sort();
        listTwo.Sort();

        var result = 0;
        for (int i = 0; i < listOne.Count; i++)
        {
            result += int.Abs(listOne[i] - listTwo[i]);
        }
        
        Console.WriteLine($"Step one result: {result}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(1, false);
        var inputList = _inputFiles.SplitString(input);
        
        var listOne = new List<int>();
        var listTwo = new List<int>();

        foreach (var inputString in inputList)
        {
            var inputValues = inputString.Split("   ");
            listOne.Add(int.Parse(inputValues[0]));
            listTwo.Add(int.Parse(inputValues[1]));
        }

        // Yes, this is probably "not the best way" but...meh.
        var result = 0;
        for (int i = 0; i < listOne.Count; i++)
        {
            var multiplicator = 0;
            for (int j = 0; j < listTwo.Count; j++)
            {
                if (listOne[i] == listTwo[j])
                {
                    multiplicator++;
                }
            }
            
            result += listOne[i] * multiplicator;
        }
        
        Console.WriteLine($"Step two result: {result}");
    }
}