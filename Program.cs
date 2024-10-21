using System;
using System.Diagnostics;

Console.CursorVisible = false;
Console.Title = "MS3D";
Console.OutputEncoding = System.Text.Encoding.UTF8;

var latticeDimensions = (x: 0, y: 0, z: 0);
int mineCount;
int flagCount;
int visibleTileCount;
Dictionary<(int x, int y, int z), (int tileData, bool isVisible, bool isFlagged)> latticeMap = new();
int view;
(int x, int y, int z) focusTile = (0, 0, 0);

Main();
void Main()
{
    Console.Clear();

    latticeDimensions = (x: 0, y: 0, z: 0);
    mineCount = 0;
    flagCount = 0;
    visibleTileCount = 0;
    view = 1;
    latticeMap.Clear();
    focusTile = (x: (latticeDimensions.x - (latticeDimensions.x % 2)) / 2, y: (latticeDimensions.y - (latticeDimensions.y % 2)) / 2, z: (latticeDimensions.z - (latticeDimensions.z % 2)) / 2);

    ConfigureLattice(ref latticeDimensions, ref mineCount);
    ConfigureLatticeData(ref latticeMap);

    Sweep();
}

void ConfigureLattice(ref (int, int, int) latticeDimensions, ref int mineCount)
{
    List<string> defaultDiffLevels = new List<string>
    {
        "Beginner      ",
        "Intermediate  ",
        "Expert        ",
        "::DEBUG:: (5,6,7)"
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
            mineCount = 10;
            break;
    }
}
void ConfigureLatticeData(ref Dictionary<(int x, int y, int z), (int tileData, bool isVisible, bool isFlagged)> latticeMap)
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

    Random mineRng = new Random();

    int placedMines = 0;

    List<(int, int, int)> mineTiles = new();

    while (placedMines < mineCount)
    {
        var randomTile = (mineRng.Next(latticeDimensions.x), mineRng.Next(latticeDimensions.y), mineRng.Next(latticeDimensions.z));

        if (latticeMap[randomTile].tileData != 9)
        {
            latticeMap[randomTile] = (9, latticeMap[randomTile].isVisible, latticeMap[randomTile].isFlagged);
            mineTiles.Add(randomTile);
            placedMines++;
        }
    }

    foreach (var mineTile in mineTiles)
    {
        var adjacentTiles = FindAdjacentTiles(mineTile);
        foreach (var adjacentTile in adjacentTiles)
        {
            if (latticeMap[adjacentTile].tileData != 9)
            {
                latticeMap[adjacentTile] = (latticeMap[adjacentTile].tileData + 1, latticeMap[adjacentTile].isVisible, latticeMap[adjacentTile].isFlagged);
            }
        }
    }

}
List<(int, int, int)> FindAdjacentTiles((int x, int y, int z) tile)
{
    List<(int, int, int)> adjacentTiles = new();

    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            for (int k = -1; k <= 1; k++)
            {
                if (latticeMap.ContainsKey((tile.x + i, tile.y + j, tile.z + k)))
                {
                    adjacentTiles.Add((tile.x + i, tile.y + j, tile.z + k));
                }
            }
        }
    }

    adjacentTiles.Remove(tile);

    return adjacentTiles;
}
void ChainClear((int x, int y, int z) tile)
{
    List<(int x, int y, int z)> chainedTiles = new();

    foreach (var adjacentTile in FindAdjacentTiles(tile))
    {
        if (!latticeMap[adjacentTile].isVisible && !latticeMap[adjacentTile].isFlagged)
        {
            latticeMap[adjacentTile] = (latticeMap[adjacentTile].tileData, true, false);
            visibleTileCount++;

            if (latticeMap[adjacentTile].tileData == 0)
            {
                chainedTiles.Add(adjacentTile);
            }
        }
    }

    foreach (var chainedTile in chainedTiles)
    {
        ChainClear(chainedTile);
    }
}
void DisplayHUD()
{
    Console.WriteLine($" Move around in the layer with [WASD] or [\u2190\u2191\u2192\u2193], move between layers with [Q] and [E].\n Press [SPACE] to destroy a tile, press [F] to place or remove a flag.");

    Console.Write(" View: ");

    for (int i = 1; i <= 3; i++)
    {
        if (i == view)
        {
            Console.Write($"{{{i}}}");
        }
        else
        {
            Console.Write($" {i} ");
        }
    }

    Console.Write($"    [*] {mineCount - flagCount}    ");

    if ((mineCount - flagCount) < 100 && (mineCount - flagCount) > -100)
    {
        Console.Write(" ");
        if ((mineCount - flagCount) < 10 && (mineCount - flagCount) > -10)
        {
            Console.Write(" ");
        }
    }

    Console.Write($"[\u2690] {flagCount}\n");
}
void DisplayLattice()
{

}

void Sweep()
{
    bool gameOver = false;
    bool gameWon = false;

    Stopwatch gameTimer = new Stopwatch();
    gameTimer.Start();

    while (!gameOver)
    {
        Console.Clear();
        DisplayHUD();
        DisplayLattice();
        Console.WriteLine($"\n ::DEBUG:: focusTile: ({focusTile.x}, {focusTile.y}, {focusTile.z})");

        ConsoleKey userInput = Console.ReadKey().Key;

        switch (userInput)
        {
            case ConsoleKey.D1:
                view = 1;
                break;
            case ConsoleKey.D2:
                view = 2;
                break;
            case ConsoleKey.D3:
                view = 3;
                break;
            case ConsoleKey.Q: //ternary operators go brrr
                focusTile = (view == 1 && focusTile.z > 0) ? (focusTile.x, focusTile.y, focusTile.z - 1) : ((view == 2 && focusTile.y > 0) ? (focusTile.x, focusTile.y - 1, focusTile.z) : ((view == 3 && focusTile.x > 0) ? (focusTile.x - 1, focusTile.y, focusTile.z) : (focusTile)));
                break;
            case ConsoleKey.E:
                focusTile = (view == 1 && focusTile.z < latticeDimensions.z - 1) ? (focusTile.x, focusTile.y, focusTile.z + 1) : ((view == 2 && focusTile.y < latticeDimensions.y - 1) ? (focusTile.x, focusTile.y + 1, focusTile.z) : ((view == 3 && focusTile.x < latticeDimensions.x - 1) ? (focusTile.x + 1, focusTile.y, focusTile.z) : (focusTile)));
                break;
            case ConsoleKey.UpArrow:
            case ConsoleKey.W:
                focusTile = (view == 1) ? ((focusTile.y > 0) ? (focusTile.x, focusTile.y - 1, focusTile.z) : focusTile) : ((focusTile.z < latticeDimensions.z - 1) ? (focusTile.x, focusTile.y, focusTile.z + 1) : (focusTile));
                break;
            case ConsoleKey.DownArrow:
            case ConsoleKey.S:
                focusTile = (view == 1) ? ((focusTile.y < latticeDimensions.y - 1) ? (focusTile.x, focusTile.y + 1, focusTile.z) : focusTile) : ((focusTile.z > 0) ? (focusTile.x, focusTile.y, focusTile.z - 1) : (focusTile));
                break;
            case ConsoleKey.LeftArrow:
            case ConsoleKey.A:
                focusTile = (view == 3) ? ((focusTile.y > 0) ? (focusTile.x, focusTile.y - 1, focusTile.z) : focusTile) : ((focusTile.x < latticeDimensions.x - 1) ? (focusTile.x + 1, focusTile.y, focusTile.z) : (focusTile));
                break;
            case ConsoleKey.RightArrow:
            case ConsoleKey.D:
                focusTile = (view == 3) ? ((focusTile.y < latticeDimensions.y - 1) ? (focusTile.x, focusTile.y + 1, focusTile.z) : focusTile) : ((focusTile.x > 0) ? (focusTile.x - 1, focusTile.y, focusTile.z) : (focusTile));
                break;
            case ConsoleKey.Spacebar:
            case ConsoleKey.Enter:
                if (!latticeMap[focusTile].isVisible && !latticeMap[focusTile].isFlagged)
                {
                    if (latticeMap[focusTile].tileData == 9)
                    {
                        gameOver = true;
                        break;
                    }
                    latticeMap[focusTile] = (latticeMap[focusTile].tileData, true, false);
                    visibleTileCount++;

                    if (latticeMap[focusTile].tileData == 0)
                    {
                        ChainClear(focusTile);
                    }
                }

                if (visibleTileCount + mineCount == latticeMap.Keys.Count)
                {
                    gameOver = true;
                    gameWon = true;
                }
                break;
            case ConsoleKey.F:
                flagCount = latticeMap[focusTile].isFlagged ? flagCount - 1 : flagCount + 1;
                latticeMap[focusTile] = (latticeMap[focusTile].tileData, latticeMap[focusTile].isVisible, !latticeMap[focusTile].isFlagged);
                break;
        }

    }

    gameTimer.Stop();
    decimal gameTimeSec = Convert.ToDecimal(gameTimer.Elapsed.Milliseconds) / 1000;

    bool quitCommand = false;

    while (!quitCommand)
    {
        if (gameWon)
        {
            Console.WriteLine($"--- WELL DONE ---\nYou have completed the game in just {gameTimeSec} seconds\nPress [R] to restart, press [Q] to quit");
        }
        else
        {
            Console.WriteLine($"--- GAME OVER ---\nYou have failed miserably in just {gameTimeSec} seconds\nPress [R] to restart, press [Q] to quit");
        }

        ConsoleKey userInput = Console.ReadKey().Key;

        switch (userInput)
        {
            case ConsoleKey.R:
                Main();
                break;
            case ConsoleKey.Q:
                quitCommand = true;
                break;
            default:
                break;
        }
    }
}