using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

	/*
	 * Tiles are 16x16 pixels.
	 * Spritesheet was originally 16 tiles high, 
	 * -> so therefore use 16 as default height.
	 * --> the top row is always 15th.
	 */

	public static SpriteManager instance;

	public Texture2D spriteSheet;
	[HideInInspector] public TileSet currentTileSet = TileSet.Concrete;

	public enum TileSet { Concrete, Shop, Sewer, Sand }

	// add new ones to the bottom, because the selection in unity
	// will shift by one and you would have to move them manually...ffs.
	public enum SpriteType { 
		// corners
		CornerTL, CornerTR, CornerBL, CornerBR,
		// floor
		Floor, FloorSpecialItem, 
		// deadends
		DeadendB, DeadendT, DeadendL, DeadendR, 
		// junctions
		JunctionT, JunctionB, JunctionL, JunctionR, 
		// horizontals
        HorizontalBottom, HorizontalTop, HorizontalBoth, 
		// verticals
        VerticalLeft, VerticalRight, VerticalBoth,
		// other
		Middle, Single, Exit,
		// doors
		DoorHorizontalClosed, DoorVerticalClosed,
		DoorHorizontalOpen, DoorVerticalOpen, 
		// traps
		TrapOff, TrapOn, 
		// chests
		ChestOpen, ChestClosed,
		// spell effects
        IceBlock,
		// GUI statuses
		GUIStatusBleeding, GUIStatusHealing, 
		GUIStatusAttBuff, GUIStatusDefBuff, GUIStatusAttDebuff, GUIStatusDefDebuff,
		GUIStatusStun, GUIStatusExpMult,
        GUIStatusBurning, GUIStatusInvulnerable,
		// actors
		PlayerSprite, Berzerker, SlimeBlue, SlimePurple, SlimeGreen, SlimeSmall, DrybonesGrey,
		DrybonesRed, Eye, FlyingSkull, ForesterBlue, ForesterGreen, Imp, MiniShroomBlue,
		MiniShroomGreen, MiniShroomRed, TentaclePurple, ShroomBlue, ShroomGreen, ShroomRed, Tiny,
		// bosses
		slimeKing,
		// items
		ArmorWood, ArmorIron, ArmorDiamond, ArmorRuby, ArmorEmerald,
		SwordWood, SwordIron, SwordDiamond, SwordRuby, SwordEmerald,
		BookRed, BookBlue, BookGreen, BookPurple, BookBlack, BookOrange,
		ScrollRed, ScrollGreen, ScrollPurple,
		Shadow, LightCircle, 
		PotionBlue, PotionRed, PotionBlack, PotionGreen, 
		PotionOrange, PotionBigRed, PotionPurple,
		GoldNugget, GoldPile, 
		Blood01, Skull01, Candle01, Lantern01,
		HammerRed, ScepterBlue, ScepterRed,
		Fireball, SkeletonBlue, SkeletonWhite, SkeletonGrey
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

		// top of the sheet is the 15th row.
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

		case SpriteType.ChestClosed:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

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

		case SpriteType.PlayerSprite:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 15);

		case SpriteType.Berzerker:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 14);

		case SpriteType.SlimeBlue:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 14);
		
		case SpriteType.SlimePurple:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 14);

		case SpriteType.SlimeGreen:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 15);

		case SpriteType.SlimeSmall:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 13);

		case SpriteType.DrybonesGrey:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 13);

		case SpriteType.DrybonesRed:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		case SpriteType.Eye:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 15);

		case SpriteType.FlyingSkull:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 14);

		case SpriteType.ForesterBlue:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.ForesterGreen:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		case SpriteType.Imp:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 14);

		case SpriteType.MiniShroomBlue:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.MiniShroomGreen:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.MiniShroomRed:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		case SpriteType.TentaclePurple:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 15);

		case SpriteType.ShroomBlue:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.ShroomGreen:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.ShroomRed:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		case SpriteType.Tiny:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 15);

		case SpriteType.ArmorWood:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.ArmorIron:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.ArmorDiamond:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 3);

		case SpriteType.ArmorRuby:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 4);

		case SpriteType.ArmorEmerald:
			rowNumber = 15 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 5);

		case SpriteType.SwordWood:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.SwordIron:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.SwordDiamond:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 3);

		case SpriteType.SwordRuby:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 4);

		case SpriteType.SwordEmerald:
			rowNumber = 14 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 5);

		case SpriteType.BookRed:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.BookBlue:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.BookGreen:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 3);

		case SpriteType.BookPurple:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 4);

		case SpriteType.BookBlack:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 5);

		case SpriteType.BookOrange:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 6);

		case SpriteType.ScrollRed:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 7);

		case SpriteType.ScrollGreen:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 8);

		case SpriteType.ScrollPurple:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 9);

		case SpriteType.Shadow:
			rowNumber = 13 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 0);

		case SpriteType.LightCircle:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 0);

		case SpriteType.PotionBlue:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.PotionRed:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.PotionBlack:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 3);

		case SpriteType.PotionGreen:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 4);

		case SpriteType.PotionOrange:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 5);

		case SpriteType.PotionBigRed:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 6);

		case SpriteType.PotionPurple:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 7);

		case SpriteType.GoldNugget:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 5);

		case SpriteType.GoldPile:
			rowNumber = 11 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 6);

		case SpriteType.Blood01:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 1);

		case SpriteType.Skull01:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.Candle01:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 3);

		case SpriteType.Lantern01:
			rowNumber = 10 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 4);

		case SpriteType.HammerRed:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 8);

		case SpriteType.ScepterBlue:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 9);

		case SpriteType.ScepterRed:
			rowNumber = 12 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.slimeKing:
			rowNumber = 8 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 13);

		case SpriteType.Fireball:
			rowNumber = 9 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 2);

		case SpriteType.SkeletonBlue:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 10);

		case SpriteType.SkeletonWhite:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 11);

		case SpriteType.SkeletonGrey:
			rowNumber = 7 + (GetTileCountHeight() - spriteSheetTileCountStandard);
			return ReadSpriteSheet(rowNumber, 12);

		default:
			Debug.LogError("No such sprite found.");
			return null;
		}

		// should not get here.
		return null;
	}
}
