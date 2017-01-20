using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

	/*
	 * Tiles are 16x16 pixels.
	 * Spritesheet was originally 16 tiles high, 
	 * -> so therefore use 16 as default height.
	 */

	public static SpriteManager instance;

	public Texture2D spriteSheet;
	[HideInInspector] public TileSet currentTileSet = TileSet.Concrete;

	public enum TileSet { Concrete, Shop, Sewer, Sand }

	public enum SpriteType { CornerTL, CornerTR, CornerBL, CornerBR, Floor, DeadendB, DeadendT, 
		DeadendL, DeadendR, JunctionT, JunctionB,
		JunctionL, JunctionR, 
        HorizontalBottom, HorizontalTop, HorizontalBoth, 
        VerticalLeft, VerticalRight, VerticalBoth,
		Middle, Single, Exit, FloorSpecialItem, 
		DoorHorizontalClosed, DoorVerticalClosed,
		DoorHorizontalOpen, DoorVerticalOpen, 
		TrapOff, TrapOn, 
		ChestOpen,
        IceBlock,
		GUIStatusBleeding, GUIStatusHealing, 
		GUIStatusAttBuff, GUIStatusDefBuff, GUIStatusAttDebuff, GUIStatusDefDebuff,
		GUIStatusStun, GUIStatusExpMult,
        GUIStatusBurning, GUIStatusInvulnerable
	}

	private Color32[] pixels;

	// base spritesheet is 16 x 16 tiles
	// if we add more tiles this will keep the old 
	// tiles not shifting down.
	// This is because for loop starts from bottom. (origin 0,0)
	private int spriteSheetTileCountStandard = 16;

	void Awake() { 
		instance = this; 
		pixels = spriteSheet.GetPixels32();
	}

	private int GetTileCountHeight() { return spriteSheet.height / 16; }
	private int GetTileCountWidth() { return spriteSheet.width / 16; }

	public void RandomizeTileSet() {

        if (GameMaster.instance.forceTileset)
        {
            currentTileSet = GameMaster.instance.forcedTileSet;
        }
        else
        {
            System.Array values = System.Enum.GetValues(typeof(TileSet));
            System.Random random = new System.Random();
            TileSet randomTileSet = (TileSet)values.GetValue(random.Next(values.Length));
            currentTileSet = randomTileSet;
        }
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

		int rowNumber = 0;

		switch(type) {
		case SpriteType.CornerBL:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 0);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 0);
			} 

			break;
		case SpriteType.CornerBR:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 2);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 2);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 2);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 2);
			} 

			break;
		case SpriteType.CornerTL:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 0);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 0);
			} 

			break;
		case SpriteType.CornerTR:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 2);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 2);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 2);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 2);
			} 

			break;
		case SpriteType.DeadendB:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 3);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 3);
			} 

			break;
		case SpriteType.DeadendL:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 3);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 3);
			} 

			break;
		case SpriteType.DeadendR:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 4);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 4);
			} 

			break;
		case SpriteType.DeadendT:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 3);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 3);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 3);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 3);
			} 

			break;
		case SpriteType.Floor:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 5);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 5);
			} 

			break;
		case SpriteType.HorizontalBottom:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 1);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 1);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 1);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 1);
			} 

			break;

        case SpriteType.HorizontalTop:

            rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 1);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 1);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 1);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 1);
			} 
            break;

        case SpriteType.HorizontalBoth:

            rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 9);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 9);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 9);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 9);
			} 
            break;

        case SpriteType.VerticalBoth:

            rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 9);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 9);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 9);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 9);
			}
            break;

		case SpriteType.VerticalLeft:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 0);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 0);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 0);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 0);
			} 

			break;

        case SpriteType.VerticalRight:

            rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 2);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 2);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 2);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 2);
			}

            break;

		case SpriteType.Single:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 4);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 4);
			} 

			break;
		case SpriteType.Middle:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 1);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 1);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 1);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 1);
			} 

			break;
		case SpriteType.JunctionB:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 6);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 6);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 6);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 6);
			} 

			break;
		case SpriteType.JunctionL:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 6);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 6);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 6);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 6);
			} 

			break;
		case SpriteType.JunctionR:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 5);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 5);
			} 

			break;
		case SpriteType.JunctionT:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 5);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 5);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 5);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 5);
			} 

			break;
		case SpriteType.Exit:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 4);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 4);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 4);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 4);
			} 

			break;
		case SpriteType.FloorSpecialItem:

			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 7);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 7);
			} 

			break;
		case SpriteType.DoorHorizontalClosed:

			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 7);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 7);
			} 
			break;

		case SpriteType.DoorVerticalClosed:
			
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 8);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 8);
			} 
			break;

		case SpriteType.DoorHorizontalOpen:
			
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 7);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 7);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 7);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 7);
			} 
			break;

		case SpriteType.DoorVerticalOpen:

			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 8);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 8);
			} 
			break;

		case SpriteType.TrapOff:
			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 8);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 8);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 8);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 8);
			} 
			break;

		case SpriteType.TrapOn:
			rowNumber = 6 + (GetTileCountHeight() - spriteSheetTileCountStandard);

			if(currentTileSet == TileSet.Concrete) {
				return ReadSpriteSheet(rowNumber, 9);
			} else if(currentTileSet == TileSet.Shop) {
				return ReadSpriteSheet(rowNumber - 3, 9);
			} else if(currentTileSet == TileSet.Sewer) {
				return ReadSpriteSheet(rowNumber - 6, 9);
			} else if(currentTileSet == TileSet.Sand) {
				return ReadSpriteSheet(rowNumber - 9, 9);
			} 
			break;

        case SpriteType.IceBlock:
            rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
            return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.ChestOpen:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.GUIStatusBleeding:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.GUIStatusHealing:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.GUIStatusAttBuff:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.GUIStatusDefBuff:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.GUIStatusAttDebuff:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.GUIStatusDefDebuff:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.GUIStatusStun:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		case SpriteType.GUIStatusExpMult:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

        case SpriteType.GUIStatusBurning:
            rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

        case SpriteType.GUIStatusInvulnerable:
            rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
            return ReadSpriteSheet(rowNumber, 11);

		default:
			Debug.LogError("No such sprite found.");
			return null;
		}

		// should not get here.
		return null;
	}
}
