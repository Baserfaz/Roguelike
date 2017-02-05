using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorToRoom {

	private static List<Tile> corridorTiles = new List<Tile>();

	public static void Generate(int height, int width) {

		corridorTiles.Clear();

		// Generate all tiles.
		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {

				// outer walls.
				if(y == 0 || y == height - 1 || x == 0 || x == width - 1) {
					DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.OuterWall);
					continue;
				}

				DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Wall);
			}
		}

		CreateCorridors ();
		CreateRooms();
	}

	private static GameObject[] GetAdjacentTiles(Vector2 pos) {
		
		// get surrounding tiles
		GameObject gUp = DungeonGenerator.instance.GetTileAtPos(pos + new Vector2(0f, 1f)); 
		GameObject gDown = DungeonGenerator.instance.GetTileAtPos(pos + new Vector2(0f, -1f));
		GameObject gLeft = DungeonGenerator.instance.GetTileAtPos(pos + new Vector2(-1f, 0f));
		GameObject gRight = DungeonGenerator.instance.GetTileAtPos(pos + new Vector2(1f, 0f));

		// set them in a list
		List<GameObject> surroundingTiles = new List<GameObject>(); 
		if(gUp != null) surroundingTiles.Add(gUp);
		if(gDown != null) surroundingTiles.Add(gDown);
		if(gLeft != null) surroundingTiles.Add(gLeft);
		if(gRight != null) surroundingTiles.Add(gRight);

		return surroundingTiles.ToArray();
	}

	private static Vector2 RandomizeDirection() {
		Vector2 dirvect = Vector2.zero;
		int dir = Random.Range (0, 4);
		if (dir == 0) dirvect = new Vector2 (0f, 1f);
		else if (dir == 1) dirvect = new Vector2(1f, 0f);
		else if (dir == 2) dirvect = new Vector2(0f, -1f);
		else if (dir == 3) dirvect = new Vector2(-1f, 0f);
		return dirvect;
	}

	private static bool CheckNextTile(Tile current, Vector2 dir) {
		Tile t = DungeonGenerator.instance.GetTileAtPos(current.position + dir).GetComponent<Tile>();
		if(t.myType == Tile.TileType.Wall) return true;
		else return false;
	}

	private static Tile RandomTileOfType(Tile.TileType type) {

		Tile tile = null;
		Tile ret = null;

		GameObject[] allTiles = DungeonGenerator.instance.GetTiles().ToArray();

		while(true) {
			tile = allTiles[Random.Range(0, allTiles.Length)].GetComponent<Tile>();
			if(tile.myType == type){
				ret = tile;
				break;
			}
		}
		return ret;
	}

	private static void CreateStraightCorridor() {
		Tile startTile = null;
		Vector2 rDir = Vector2.zero;

		while(startTile == null) {
			// create straight corridors
			Tile oWall = RandomTileOfType(Tile.TileType.OuterWall);

			// randomize direction
			rDir = RandomizeDirection();

			GameObject nextTileGo = DungeonGenerator.instance.GetTileAtPos(oWall.position + rDir);

			if(nextTileGo != null) {

				Tile nextTile = nextTileGo.GetComponent<Tile>();

				if(nextTile.myType == Tile.TileType.Wall) {
					startTile = nextTile;
					break;
				}
			}
		}

		Tile current = startTile;

		while(true) {
			corridorTiles.Add(current);
			DungeonGenerator.instance.UpdateTileType(current, Tile.TileType.Floor);
			current = DungeonGenerator.instance.GetTileAtPos(current.position + rDir).GetComponent<Tile>();
			if(current.myType == Tile.TileType.OuterWall) break;
		}
	}

	private static void CreateCorridors() {

		for(int i = 0; i < 5; i++) {
			CreateStraightCorridor();
		}
	}

	private static void CreateRoom() {


		// room creation algorithm
		// 1. take random floor tile
		// 2. randomize room size
		// 3. randomize room offset
		// 4. create room

		Tile rFloor = corridorTiles[Random.Range(0, corridorTiles.Count)];

		Vector2 offset = new Vector2(0, 0);

		int r = Random.Range(3, 10);

		int height = r;
		int width = r;

		for(int y = 0; y < height; y++) {
			for(int x = 0; x < width; x++) {

				GameObject g = DungeonGenerator.instance.GetTileAtPos(rFloor.position + new Vector2(x, y) + offset);

				if(g != null) {
					Tile gTile = g.GetComponent<Tile>();

					if(gTile.myType != Tile.TileType.OuterWall) {
						DungeonGenerator.instance.UpdateTileType(gTile, Tile.TileType.Floor);
					}
				}
			}
		}
	}

	private static void CreateRooms() {
		for(int i = 0; i < 15; i++) {
			CreateRoom();
		}
	}
}
