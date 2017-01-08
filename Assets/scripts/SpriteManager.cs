using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

	/*
	 * Tiles are 16x16 pixels.
	 * 
	 * 
	 */

	public static SpriteManager instance;

	public Texture2D spriteSheet;
	public TileSet currentTileSet = TileSet.Concrete;

	public enum TileSet { Concrete, Shop, Sewer }
	public enum SpriteType { CornerTL, CornerTR, CornerBL, CornerBR, Floor, DeadendB, DeadendT, DeadendL, DeadendR, JunctionT, JunctionB,
		JunctionL, JunctionR, Horizontal, Vertical, Middle, Single, Exit, FloorSpecialItem, DoorHorizontalClosed, DoorVerticalClosed,
		DoorHorizontalOpen, DoorVerticalOpen, TrapOff, TrapOn }

	private Color32[] pixels;

	// base spritesheet is 16 x 16 tiles
	// if we add more tiles this will keep the old 
	// tiles not shifting down.
	// This is because for loop starts from bottom. (origin 0,0)
	private int spriteSheetTileCountStandard = 16;

	void Awake() { instance = this; } 
	void Start() { pixels = spriteSheet.GetPixels32(); }

	private int GetTileCountHeight() { return spriteSheet.height / 16; }
	private int GetTileCountWidth() { return spriteSheet.width / 16; }

	public void RandomizeTileSet() {

		System.Array values = System.Enum.GetValues(typeof(TileSet));
		System.Random random = new System.Random();
		TileSet randomTileSet = (TileSet)values.GetValue(random.Next(values.Length));

		currentTileSet = randomTileSet;
	}


	private Sprite ReadSpriteSheet(int row, int column) {
		List<Color32> spriteColors = new List<Color32>();

		// reads the spritesheet from bottom left corner to top right corner.
		// row = 0 && column = 0 is bottom left corner.

		for(int y = row * 16; y < (row + 1) * 16; y++) {
			for(int x = column * 16; x < (column + 1) * 16; x++) {

				Color32 currentPixel = pixels[(y * spriteSheet.width) + x];
				spriteColors.Add(currentPixel);

			}
		}

		// create texture
		Color32[] spriteData = spriteColors.ToArray();
		Texture2D spriteTex = new Texture2D(16, 16);

		spriteTex.SetPixels32(spriteData);
		spriteTex.Apply();

		// set filter mode
		spriteTex.filterMode = FilterMode.Point;

		// return new sprite.
		return Sprite.Create(spriteTex, new Rect(0, 0, spriteTex.width, spriteTex.height), new Vector2(0.5f, 0.5f), 16f);

	}

	/// <summary>
	/// Reads the spritesheet from given row and column and creates texture2D and converts it to sprite.
	/// rows and columns example can be found in the project files.
	/// </summary>
	/// <returns>The texture.</returns>
	/// <param name="type">Type.</param>
	public Sprite CreateTexture(SpriteType type) {

		int topTilePos = 0;

		switch(type) {
		case SpriteType.CornerBL:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 0);
			}

			break;
		case SpriteType.CornerBR:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 2);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 2);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 2);
			}

			break;
		case SpriteType.CornerTL:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 0);
			}

			break;
		case SpriteType.CornerTR:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 2);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 2);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 2);
			}

			break;
		case SpriteType.DeadendB:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 3);
			}

			break;
		case SpriteType.DeadendL:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 3);
			}

			break;
		case SpriteType.DeadendR:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 4);
			}

			break;
		case SpriteType.DeadendT:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 3);
			}

			break;
		case SpriteType.Floor:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 5);
			}

			break;
		case SpriteType.Horizontal:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 1);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 1);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 1);
			}

			break;
		case SpriteType.Vertical:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 0);
			}

			break;
		case SpriteType.Single:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 4);
			}

			break;
		case SpriteType.Middle:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 1);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 1);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 1);
			}

			break;
		case SpriteType.JunctionB:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 6);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 6);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 6);
			}

			break;
		case SpriteType.JunctionL:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 6);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 6);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 6);
			}

			break;
		case SpriteType.JunctionR:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 5);
			}

			break;
		case SpriteType.JunctionT:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 5);
			}

			break;
		case SpriteType.Exit:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 4);
			}
			break;
		case SpriteType.FloorSpecialItem:

			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 7);
			}
			break;
		case SpriteType.DoorHorizontalClosed:

			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 7);
			}
			break;

		case SpriteType.DoorVerticalClosed:
			
			topTilePos = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 8);
			}
			break;
		case SpriteType.DoorHorizontalOpen:
			
			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 7);
			}
			break;
		case SpriteType.DoorVerticalOpen:

			topTilePos = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 8);
			}
			break;
		case SpriteType.TrapOff:
			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 8);
			}
			break;
		case SpriteType.TrapOn:
			topTilePos = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(topTilePos, 9);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(topTilePos - 3, 9);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(topTilePos - 6, 9);
			}
			break;
		}

		// should never get here.
		return null;
	}


}
