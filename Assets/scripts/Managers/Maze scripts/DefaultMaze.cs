using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMaze {

    public static void Generate(int height, int width) {

        // Generate all tiles.
		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {

				// outer walls.
				if(y == 0 || y == height - 1 || x == 0 || x == width - 1) {
					DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.OuterWall);
					continue;
				}
					
				// create walls.
				int rand = Random.Range(1, Mathf.FloorToInt(GameMaster.instance.dungeonSpaciousness * 10f)); 
				if(x % rand == 0 || y % rand == 0) {
                    DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Wall);
					continue;
				}

				// create floor.
                DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Floor);
			}
		}
    }
}
