Console.CursorVisible = false;
Console.Title = "Minesweeper 3D";
Console.OutputEncoding = System.Text.Encoding.UTF8;

var latticeDimensions = (x: 0, y: 0, z: 0);
int mineCount = 0;
ConfigureLattice(ref latticeDimensions, ref mineCount);
Console.Clear();

int tileCount = latticeDimensions.Item1 * latticeDimensions.Item2 * latticeDimensions.Item3;
Dictionary<int, int> latticeMap = new Dictionary<int, int>();
ConfigureLatticeData(tileCount, mineCount, ref latticeMap);

List<int> visibleTiles = new List<int>();

LatticeDisplayConfig topView = new LatticeDisplayConfig();
SetDisplayConfig(ref topView, 1);
LatticeDisplayConfig frontView = new LatticeDisplayConfig();
SetDisplayConfig(ref frontView, 2);
LatticeDisplayConfig leftView = new LatticeDisplayConfig();
SetDisplayConfig(ref leftView, 3);


DisplayLattice(leftView, 0);



void SetDisplayConfig(ref LatticeDisplayConfig config, int view)
{
    config.view = view;

    switch (view)
    {
        case 1:
            break;
        case 2:
            break;
        case 3:
            List<char> hTiles = new List<char>()
                {
                    '/'
                };
            List<char> edge = new List<char>();
            for (int i = 0; i < latticeDimensions.y; i++)
            {
                hTiles.AddRange(new List<char>
                    {
                        '_','_','_','/'
                    });
                edge.AddRange(new List<char>
                    {
                        ' ', '_', '_', '_'
                    });
            }
            config.horizontalTiles = new string(hTiles.ToArray());
            config.edge = new string(edge.ToArray());
            config.verticalTiles = new char[]
            {
                '|','/'
            };
            break;
    }

    List<char> topLine = new List<char>
    {
        '\u2554','\u2550','\u2550','\u2550'
    };
    List<char> midLine = new List<char>
    {
        '\u255F','\u2500','\u2500','\u2500'
    };
    List<char> bottomLine = new List<char>
    {
        '\u255A','\u2550','\u2550','\u2550'
    };
    List<char> dataLine = new List<char>
    {
        '\u2551'
    };
    for (int i = 0; i < config.grid.gridWidth - 1; i++)
    {
        topLine.AddRange(new List<char>
        {
            '\u2564','\u2550','\u2550','\u2550'
        });
        midLine.AddRange(new List<char>
        {
            '\u253C','\u2500','\u2500','\u2500'
        });
        bottomLine.AddRange(new List<char>
        {
            '\u2567','\u2550','\u2550','\u2550'
        });
        dataLine.Add('\u2571');
    }
    topLine.Add('\u2557');
    midLine.Add('\u2562');
    bottomLine.Add('\u255D');
    dataLine.Add('\u2551');

    config.grid.topLine = new string(topLine.ToArray());
    config.grid.midLine = new string(midLine.ToArray());
    config.grid.bottomLine = new string(bottomLine.ToArray());
    config.grid.dataLine = dataLine.ToArray();

    config.grid.gridHeight = config.view == 1 ? latticeDimensions.y : latticeDimensions.z;
    config.grid.gridHeight = config.grid.gridHeight * 2 + 1;
    config.grid.gridWidth = config.view == 3 ? latticeDimensions.y : latticeDimensions.x;
    config.grid.gridWidth = config.grid.gridWidth * 4 + 1;
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
            for (int j = 0; j < adjacentTiles.Count; j++)
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
    var tileCoords = FindTileCoordinates(tileIndex, latticeDimensions);

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
        if (tileCoords.y != latticeDimensions.y - 1)
        {
            tileSet.Add(tileSet[i] + latticeDimensions.x);
            tempCount++;
        }
        if (tileCoords.y != 0)
        {
            tileSet.Add(tileSet[i] - latticeDimensions.x);
            tempCount++;
        }
    }

    tileSetCount += tempCount;
    for (int i = 0; i < tileSetCount; i++)
    {
        if (tileCoords.x != latticeDimensions.x - 1)
        {
            tileSet.Add(tileSet[i] + 1);
        }
        if (tileCoords.x != 0)
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
    return tileSet;
}
(int x, int y, int z) FindTileCoordinates(int tileIndex, (int x, int y, int z) latticeDimensions)
{
    var tileCoords = (x: 0, y: 0, z: 0);
    tileCoords.x = tileIndex % latticeDimensions.x;
    tileCoords.y = ((tileIndex - (tileIndex % latticeDimensions.x)) % (latticeDimensions.x * latticeDimensions.y)) / latticeDimensions.x; // (:
    tileCoords.z = (tileIndex - (tileIndex % (latticeDimensions.x * latticeDimensions.y))) / (latticeDimensions.x * latticeDimensions.y);
    return tileCoords;
}
void DisplayLattice(LatticeDisplayConfig config, int focusTile)
{

    var focusTileCoords = FindTileCoordinates(focusTile, latticeDimensions);

    switch (config.view)
    {
        case 1:
            break;
        case 2:
            break;
        case 3:
            string indent = "         ";
            int vTilesWidth = 1;

            for (int i = 0; i < (latticeDimensions.x - focusTileCoords.x); i++)
            {
                Console.Write(" ");
            }
            Console.WriteLine(indent + config.edge);

            for (int i = (latticeDimensions.x - (focusTileCoords.x + 1)); i >= 0; i--)
            {
                Console.Write(indent);
                for (int j = 0; j < i; j++)
                {
                    Console.Write(" ");
                }
                Console.Write($"{config.horizontalTiles}");

                for (int j = 0; j < vTilesWidth; j++)
                {
                    Console.Write(config.verticalTiles[j % 2]);
                }
                vTilesWidth++;
                Console.WriteLine();
            }

            break;
    }

    for (int i = 0; i < config.grid.gridHeight; i++)
    {
        Console.Write($"\n");
        DisplayGridLine(i, latticeDimensions, leftView, 0, latticeMap, visibleTiles);
    }
}
void DisplayGridLine(int line, (int x, int y, int z) latticeDimensions, LatticeDisplayConfig config, int focusTile, Dictionary<int, int> latticeMap, List<int> visibleTiles)
{


    var tileBackground = (left: ' ', right: ' ');
    var focusTileBrackets = (left: '{', right: '}');
    var invisibleTileBrackets = (left: '[', right: ']');

    char tileCenter = ' ';

    if (line == 0)
    {
        Console.Write(config.grid.topLine);
    }
    else if (line == config.grid.gridHeight - 1)
    {
        Console.Write(config.grid.bottomLine);
    }
    else if (line % 2 == 0)
    {
        Console.Write(config.grid.midLine);
    }
    else
    {
        int j = 0;
        for (int i = 0; i < config.grid.gridWidth; i++)
        {
            Console.Write(config.grid.dataLine[j++]);
            if (!visibleTiles.Contains(focusTile))
            {
                tileBackground = invisibleTileBrackets;
            }
            else if (true)
            {

            }
            Console.Write(tileBackground.left + tileCenter + tileBackground.right);
        }
        Console.Write(config.grid.dataLine.Last());
    }
}
struct LatticeDisplayConfig
{
    internal int view;
    internal char[] verticalTiles;
    internal string horizontalTiles;
    internal string edge;

    internal GridDisplayConfig grid;

}
struct GridDisplayConfig
{
    internal string topLine;
    internal string bottomLine;
    internal string midLine;
    internal char[] dataLine;
    internal int gridWidth;
    internal int gridHeight;
}

