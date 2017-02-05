using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMaze {

	private static float coverage = 0f;
	private static int floorCount = 0;
	private static List<Room> AllRooms = new List<Room>();

	public static void Generate(int width, int height) {

		// reset 
		coverage = 0f;
		floorCount = 0;
		AllRooms.Clear ();

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
		// subtract the outerwalls.
		int tileCountTotal = (width * height) - (2 * width + 2 * height - 4);

		int loop = 0;

		// main loop.
		while(coverage < GameMaster.instance.dungeonSpaciousness) {

			if (loop > 1000) {
				Debug.LogError ("infinite loop!");
				break;
			}

			// get a random tile.
			GameObject tileGo = tiles [Random.Range (0, tiles.Length)];
			Tile tile = tileGo.GetComponent<Tile> ();

			if (tile.myType == Tile.TileType.Wall) {

				// randomize room size
				int roomWidth = Random.Range(3, 5);
				int roomHeight = Random.Range(3, 5);

				// create room
				CreateRoom(tile.position, roomHeight, roomWidth);
			}

			// calculate the floor coverage.
			// it should be in a range of 0f - 1f (0% - 100%).
			coverage = (float)floorCount / (float)tileCountTotal;

			loop++;
		}

		CreatePathways ();
	}

	private static void CreatePathways() {

		// we have populated our list of rooms.
		foreach (Room r in AllRooms) {

			int count = r.roomTiles.Count;

			if (count == 0) {
				continue;
			}

			// get a random tile from a room
			Tile randTile = r.roomTiles [Random.Range (0, r.roomTiles.Count)];

			int dir = Random.Range (0, 4);
			Vector2 dirVector = Vector2.zero;

			// randomize the direction
			switch (dir) {
			case 0:
				dirVector = new Vector2 (0f, 1f);
				break;

			case 1:
				dirVector = new Vector2 (1f, 0f);
				break;

			case 2:
				dirVector = new Vector2 (0f, -1f);
				break;

			case 3:
				dirVector = new Vector2 (-1f, 0f);
				break;
			}

			for (int i = 0; i < 10; i++) {
				Tile tile = DungeonGenerator.instance.GetTileAtPos (
					randTile.position + dirVector * i).GetComponent<Tile> ();

				if (tile == null ||
					tile.myType == Tile.TileType.OuterWall ||
					tile.myType == Tile.TileType.Floor) {
					break;
				}

				DungeonGenerator.instance.UpdateTileType (tile, Tile.TileType.Floor);
			}
		}
	}

	private static void CreateRoom(Vector2 pos, int height, int width) {

		List<Tile> currentRoomTiles = new List<Tile> ();

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
				if (tile.myType == Tile.TileType.Floor ||
				   tile.myType == Tile.TileType.OuterWall) {
					continue;
				}

				// update the tile to be floor.
				DungeonGenerator.instance.UpdateTileType (tile, Tile.TileType.Floor);

				// add room to our list 
				currentRoomTiles.Add(tile);

				// add floor count.
				floorCount++;
			}

		}
		// create new room
		Room room = new Room ("Room", currentRoomTiles);

		// add it to the list of rooms.
		AllRooms.Add (room);
	}
}
