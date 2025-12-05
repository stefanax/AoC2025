namespace AoC2024;

public class Day4
{
    private readonly InputFiles _inputFiles = new InputFiles();

    public void Step1()
    {
        var input = _inputFiles.ReadInputFileForDay(4, false);
        var inputList = _inputFiles.SplitString(input);

        var lonelyAtCount = 0;

        for (var row = 0; row < inputList.Length; row++)
        {
            var line = inputList[row];

            for (var col = 0; col < line.Length; col++)
            {
                if (line[col] != '@')
                {
                    continue;
                }

                var neighborCount = CountAdjacentAts(inputList, row, col);

                if (neighborCount < 4)
                {
                    lonelyAtCount++;
                }
            }
        }

        Console.WriteLine($"Step one result: {lonelyAtCount}");
    }

    public void Step2()
    {
        var input = _inputFiles.ReadInputFileForDay(4, false);
        var inputList = _inputFiles.SplitString(input);
        var grid = inputList.Select(line => line.ToCharArray()).ToList();

        var totalRemoved = 0;

        while (true)
        {
            var lonelyAts = new List<(int Row, int Col)>();

            for (var row = 0; row < grid.Count; row++)
            {
                var line = grid[row];

                for (var col = 0; col < line.Length; col++)
                {
                    if (line[col] != '@')
                    {
                        continue;
                    }

                    var neighborCount = CountAdjacentAts(grid, row, col);

                    if (neighborCount < 4)
                    {
                        lonelyAts.Add((row, col));
                    }
                }
            }

            if (lonelyAts.Count == 0)
            {
                break;
            }

            foreach (var (row, col) in lonelyAts)
            {
                grid[row][col] = '.';
            }

            totalRemoved += lonelyAts.Count;
        }

        Console.WriteLine($"Step two result: {totalRemoved}");
    }

    private static int CountAdjacentAts(IReadOnlyList<string> grid, int row, int col)
    {
        var neighborCount = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                {
                    continue;
                }

                var newRow = row + dr;
                var newCol = col + dc;

                if (newRow < 0 || newRow >= grid.Count)
                {
                    continue;
                }

                var line = grid[newRow];

                if (newCol < 0 || newCol >= line.Length)
                {
                    continue;
                }

                if (line[newCol] == '@')
                {
                    neighborCount++;
                }
            }
        }

        return neighborCount;
    }

    private static int CountAdjacentAts(IReadOnlyList<char[]> grid, int row, int col)
    {
        var neighborCount = 0;

        for (var dr = -1; dr <= 1; dr++)
        {
            for (var dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0)
                {
                    continue;
                }

                var newRow = row + dr;
                var newCol = col + dc;

                if (newRow < 0 || newRow >= grid.Count)
                {
                    continue;
                }

                var line = grid[newRow];

                if (newCol < 0 || newCol >= line.Length)
                {
                    continue;
                }

                if (line[newCol] == '@')
                {
                    neighborCount++;
                }
            }
        }

        return neighborCount;
    }
}
