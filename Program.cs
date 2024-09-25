Console.CursorVisible = false;
Console.Title = "Minesweeper 3D";
Console.OutputEncoding = System.Text.Encoding.UTF8;
//TODO: configure a display buffer


var latticeDimensions = (0, 0, 0);
int mineCount = 0;
ConfigureLattice(ref latticeDimensions, ref mineCount);

int tileCount = latticeDimensions.Item1 * latticeDimensions.Item2 * latticeDimensions.Item3;

Dictionary<int, int> latticeMap = new Dictionary<int, int>();
ConfigureLatticeData(tileCount, mineCount, ref latticeMap);

Console.Clear();
for (int i = 0; i < latticeMap.Count; i++)
{
    if (latticeMap.ContainsKey(i))
    {
        Console.Write($"{latticeMap[i]} ");
    }
}


void ConfigureLattice(ref (int, int, int) latticeDimensions, ref int mineCount)
{
    List<string> defaultDiffLevels = new List<string>
    {
        "Beginner      ",
        "Intermediate  ",
        "Expert        "
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
    }
}
void ConfigureLatticeData(int tileCount, int mineCount, ref Dictionary<int, int> latticeMap)
{
    //0-8 for tile numbers, 9 for mine {latticeMap.value}
    Random rng = new Random();

    int iteration = 0;

    while (iteration < mineCount)
    {
        int randomTile = rng.Next(tileCount);
        if (!latticeMap.ContainsKey(randomTile))
        {
            latticeMap.Add(randomTile, 9);
            iteration++;
        }
    }

    for (int i = 0; i < tileCount; i++)
    {
        if (!latticeMap.ContainsKey(i))
        {
            List<int> adjacentTiles = FindAdjacentTiles(i, latticeDimensions);
            int tileNumber = 0;
            for (int j = 0; j < FindAdjacentTiles(i, latticeDimensions).Count; j++)
            {
                if (latticeMap.ContainsKey(adjacentTiles[j]) && latticeMap[adjacentTiles[j]] == 9)
                {
                    tileNumber++;
                }
            }
            latticeMap.Add(i, tileNumber);
        }
    }
}
List<int> FindAdjacentTiles(int tileIndex, (int x, int y, int z) latticeDimensions)
{
    List<int> tileSet = new List<int>
    {
        tileIndex,
        tileIndex + latticeDimensions.x * latticeDimensions.y,
        tileIndex - latticeDimensions.x * latticeDimensions.y
    };
    
    int tileSetCount = 3;
    int tempCount = 0;
    for (int i = 0; i < tileSetCount; i++)
    {
        if (((tileIndex - (tileIndex % latticeDimensions.x)) % (latticeDimensions.x * latticeDimensions.y)) / latticeDimensions.x != latticeDimensions.y - 1) // (:
        {
            tileSet.Add(tileSet[i] + latticeDimensions.x);
            tempCount++;
        }
        if (((tileIndex - (tileIndex % latticeDimensions.x)) % (latticeDimensions.x * latticeDimensions.y)) / latticeDimensions.x != 0)
        {
            tileSet.Add(tileSet[i] - latticeDimensions.x);
            tempCount++;
        }       
    }

    tileSetCount += tempCount;
    for (int i = 0; i < tileSetCount; i++)
    {
        if (tileIndex % latticeDimensions.x != latticeDimensions.x - 1)
        {
            tileSet.Add(tileSet[i] + 1);
        }
        if (tileIndex % latticeDimensions.x != 0)
        {
            tileSet.Add(tileSet[i] - 1);       
        }
    }

    tileSet.Remove(tileIndex);

    int tileCount = latticeDimensions.x * latticeDimensions.y * latticeDimensions.z;

    for (int i = 0; i < tileSet.Count; i++)
    {
        if (tileSet[i] < 0 || tileSet[i] > (tileCount - 1))
        {
            tileSet.RemoveAt(i);
            i--;
        }
    }

    //tileSet.Sort(); not really needed
    return tileSet;
}
