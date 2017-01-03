using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorToTileType {
	public Color32 color;
	public MapReader.PngTileType type;
}

public class MapReader : MonoBehaviour {

	public enum PngTileType { Floor, Wall, OuterWall, Exit, Start, Gold, ShopItem }
	public enum State { Tile, ItemActor }

	private State myState = State.Tile;

	public static MapReader instance;
	public ColorToTileType[] colorToTileType; 

	void Awake() { instance = this; }

	public void GenerateDungeonFromImage(Texture2D level) {

		myState = State.Tile;

		Color32[] pixels = level.GetPixels32();
		int width = level.width;
		int height = level.height;

		// loop through twice.
		// 1. create tiles (even under items & actors)
		// 2. create items and actors
		for(int i = 0; i < 2; i++) {

			// create tiles.
			for(int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					CreateTile(x, y, pixels[(y * width) + x]);
				}
			}

			// change the state to items and actors 
			// after first loop.
			myState = State.ItemActor;
		}

		DungeonGenerator.instance.CalculateWallTileSprites();
	}

	private void CreateTile(int x, int y, Color32 color) {
		foreach(ColorToTileType ctt in colorToTileType) {

			// on tile state
			// create only tiles.
			if(myState == State.Tile) {
				if(ctt.color.Equals(color)) {
					switch(ctt.type) {
					case PngTileType.Wall:
						DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Wall);
						break;
					case PngTileType.OuterWall:
						DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.OuterWall);
						break;
					case PngTileType.Exit:
						DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Exit);
						break;
					case PngTileType.Floor:
						DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Floor);
						break;
					
						// all other cases create floor also.
					default:
						DungeonGenerator.instance.CreateTile(x, y, Tile.TileType.Floor);
						break;
					}
				} 

				// create items & actors.
			} else if(myState == State.ItemActor) {

				if(ctt.color.Equals(color)) {
					switch(ctt.type) {
					case PngTileType.Start:
						PrefabManager.instance.MoveActorToPos(new Vector2(x, y), PrefabManager.instance.GetPlayerInstance());
						break;
					case PngTileType.Gold:
						PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Gold, new Vector2(x, y), Item.Rarity.Normal);
						break;
					case PngTileType.ShopItem:

						System.Array arr = System.Enum.GetValues(typeof(Item.Type));
						Item.Type randomType = (Item.Type) arr.GetValue(Random.Range(0, arr.Length));

						PrefabManager.instance.InstantiateRandomItemInCategory(randomType, new Vector2(x, y), Item.Rarity.Normal, true);
						break;
					default:
						break;
					}
				} 

			}
		}
	}

}
