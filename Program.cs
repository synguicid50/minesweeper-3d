Console.CursorVisible = false;
Console.Title = "Minesweeper 3D";
Console.OutputEncoding = System.Text.Encoding.UTF8;

var latticeDimensions = (x: 0, y: 0, z: 0);
int mineCount = 0;

ConfigureLattice(ref latticeDimensions, ref mineCount);
Console.Clear();

Dictionary<(int x, int y, int z), (int tileData, bool isVisible, bool isFlagged)> latticeMap = new();


void ConfigureLattice(ref (int, int, int) latticeDimensions, ref int mineCount)
{
    List<string> defaultDiffLevels = new List<string>
    {
        "Beginner      ",
        "Intermediate  ",
        "Expert        ",
        "TestDimensions (5,6,7)"
    };

    bool inputReceived = false;
    int arrowPos = 0;

    while (!inputReceived)
    {
        Console.Clear();
        Console.Write("Choose difficulty:");

        for (int i = 0; i < defaultDiffLevels.Count; i++)
        {
            Console.Write($"\n{defaultDiffLevels[i]}");
            if (arrowPos == i)
            {
                Console.Write("\u2190");
            }
        }

        ConsoleKey userInput = Console.ReadKey().Key;

        switch (userInput)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                if (arrowPos > 0)
                {
                    arrowPos--;
                }
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                if (arrowPos < defaultDiffLevels.Count - 1)
                {
                    arrowPos++;
                }
                break;
            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter:
                inputReceived = true;
                break;
        }
    }

    switch (arrowPos)
    {
        case 0:
            latticeDimensions = (5, 5, 5);
            mineCount = 10;
            break;
        case 1:
            latticeDimensions = (7, 7, 7);
            mineCount = 33;
            break;
        case 2:
            latticeDimensions = (9, 9, 9);
            mineCount = 99;
            break;
        case 3:
            latticeDimensions = (5, 6, 7);
            mineCount = 25;
            break;
    }
}

void ConfigureTileData(ref Dictionary<(int x, int y, int z), (int tileData, bool isVisible, bool isFlagged)> latticeMap)
{
    for (int i = 0; i < latticeDimensions.x; i++)
    {
        for (int j = 0; j < latticeDimensions.y; j++)
        {
            for (int k = 0; k < latticeDimensions.z; k++)
            {
                latticeMap.Add((i, j, k), (0, false, false));
            }
        }
    }
}