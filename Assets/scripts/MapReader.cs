using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColorToTileType {
	public Color32 color;
	public Tile.TileType type;
}

public class MapReader : MonoBehaviour {
		
	public static MapReader instance;
	public ColorToTileType[] colorToTileType; 

	void Awake() { instance = this; }

	public void GenerateDungeonFromImage(Texture2D level) {

		Color32[] pixels = level.GetPixels32();
		int width = level.width;
		int height = level.height;

		for(int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				CreateTile(x, y, pixels[(y * width) + x]);
			}
		}

		DungeonGenerator.instance.CalculateWallTileSprites();
	}

	private void CreateTile(int x, int y, Color32 color) {
		foreach(ColorToTileType ctt in colorToTileType) {
			if(ctt.color.Equals(color)) DungeonGenerator.instance.CreateTile(x, y, ctt.type);
		}
	}

}
