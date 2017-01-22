using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMaze {

	private static float coverage = 0f;
	private static int floorCount = 0;

	public static void Generate(int width, int height) {

		// reset 
		coverage = 0f;
		floorCount = 0;

		// Populate the dungeon with walls
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				// populate empty dungeon.
				if (y == 0 || y == height - 1 || x == 0 || x == width - 1) {

					// create outer walls.
					DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.OuterWall);
				} else {
					// otherwise create wall.
					DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Wall);
				}
			}
		}

		// simple room creation algorithm:
		// 1. takes a random wall tile.
		// 2. randomizes room's width and height values.
		// 3. creates that size room by changing 
		//    those wall tiles to floor tiles.
		// 4. Do this multiple times 
		// 5. create doors somehow..

		// reference all tiles.
		GameObject[] tiles = DungeonGenerator.instance.GetTiles().ToArray();

		// there this many tiles in total.
		int tileCountTotal = width * height;

		int loop = 0;

		while(coverage < GameMaster.instance.dungeonSpaciousness) {

			if (loop > 100) {
				Debug.LogError ("while true");
				break;
			}

			// get a random tile.
			GameObject tileGo = tiles [Random.Range (0, tiles.Length)];
			Tile tile = tileGo.GetComponent<Tile> ();

			if (tile.myType == Tile.TileType.Wall) {

				// randomize room size
				int roomWidth = Random.Range(2, 5);
				int roomHeight = Random.Range(2, 5);

				// create room
				CreateRoom(tile.position, roomHeight, roomWidth);
			}

			// calculate the floor coverage.
			// it should be in a range of 0f - 1f (0% - 100%).
			coverage = (float)floorCount / (float)tileCountTotal;

			loop++;
		}
	}

	private static void CreateRoom(Vector2 pos, int height, int width) {

		// loop through the tiles and 
		// create floor.
		// -> this is the "room".
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				// get the current tile gameobject.
				GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(
					pos + new Vector2(x, y));

				// null check 
				// if the tile is out of bounds.
				if (tileGo == null) continue;

				// get tile.
				Tile tile = tileGo.GetComponent<Tile> ();

				// check if the current tile is not
				// a floor or an outerwall.
				if(tile.myType == Tile.TileType.Floor || 
					tile.myType == Tile.TileType.OuterWall) continue;

				// update the tile to be floor.
				DungeonGenerator.instance.UpdateTileType (tile, Tile.TileType.Floor);

				// add floor count.
				floorCount++;
			}
		}
	}

}
