using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMazeBuilder {

    static Tile currentTile = null;
    static int floorCount = 0;
    static List<Tile> visited = new List<Tile>();

    private static Tile GetRandomTile(int width, int height, Tile.TileType type)
    {
        while (true)
        {
            Vector2 startPoint = new Vector2(Random.Range(1, width - 1), Random.Range(1, height - 1));
            GameObject tileGO = DungeonGenerator.instance.GetTileAtPos(startPoint);
            Tile tile = tileGO.GetComponent<Tile>();

            if (tile.myType == type)
            {
                currentTile = tile;
                break;
            }
        }

        return currentTile;
    }

    private static void CreateFloor()
    {
        // randomize direction.
        // 0 = up, 1 = right, 2 = down, 3 = left
        int dir = Random.Range(0, 4);

        // randomize length
        int length = Random.Range(10, 20);

        // create floors.
        for (int i = 1; i <= length; i++)
        {

            GameObject go = null;

            // get the new tile
            // 0 = up, 1 = right, 2 = down, 3 = left
            switch (dir)
            {
                case 0:
                    go = DungeonGenerator.instance.GetTileAtPos(currentTile.position + new Vector2(0f, i));
                    break;

                case 1:
                    go = DungeonGenerator.instance.GetTileAtPos(currentTile.position + new Vector2(i, 0f));
                    break;

                case 2:
                    go = DungeonGenerator.instance.GetTileAtPos(currentTile.position - new Vector2(0f, i));
                    break;

                case 3:
                    go = DungeonGenerator.instance.GetTileAtPos(currentTile.position - new Vector2(i, 0f));
                    break;
            }

            // if its valid 
            if (go == null) break;

            if (go.GetComponent<Tile>().myType == Tile.TileType.OuterWall || go.GetComponent<Tile>().myType == Tile.TileType.Floor)
            {
                break;
            }

            // update tile to floor.
            DungeonGenerator.instance.UpdateTileType(go.GetComponent<Tile>(), Tile.TileType.Floor);
            visited.Add(go.GetComponent<Tile>());

            // add counters.
            floorCount++;
        }
    }

    public static void Generate(int height, int width)
    {

        int maxFloorAmount = 0;
        int wallCount = 0;

        // reset
        floorCount = 0;
        visited.Clear();
        currentTile = null;

        // populate the dungeon with walls
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                // outer walls.
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.OuterWall);
                }
                else
                {
                    // create normal wall.
                    DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Wall);
                    wallCount++;
                }
            }
        }

        // how much floor are there?
        maxFloorAmount = Mathf.FloorToInt(wallCount * GameMaster.instance.dungeonSpaciousness);

        // create first tile from a random wall tile.
        currentTile = GetRandomTile(width, height, Tile.TileType.Wall);
        CreateFloor();

        // Create floor here
        while (floorCount < maxFloorAmount)
        {
            // get a visited tile.
            currentTile = visited[Random.Range(0, visited.Count)];

            // create floor to some random direction.
            CreateFloor();
        }
    }
}
