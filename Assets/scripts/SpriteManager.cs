using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

	public static SpriteManager instance;

	public enum TileSet { Simple, Shop }
	public enum SpriteType { CornerTL, CornerTR, CornerBL, CornerBR, Floor, DeadendB, DeadendT, DeadendL, DeadendR, JunctionT, JunctionB,
		JunctionL, JunctionR, Horizontal, Vertical, Middle, Single }

	// set this to get correct tileset.
	public TileSet currentTileSet = TileSet.Simple;

	[Header("Common tiles")]
	public Sprite s_exit;

	[Space(25)]

	[Header("Simple Tileset Sprites")]
	[Header("Floor sprite")]
	public Sprite simple_floor;
	[Header("Corners")]
	public Sprite simple_wall_corner_top_left;
	public Sprite simple_wall_corner_top_right;
	public Sprite simple_wall_corner_bottom_left;
	public Sprite simple_wall_corner_bottom_right;
	[Header("Deadends")]
	public Sprite simple_wall_deadend_bottom;
	public Sprite simple_wall_deadend_top;
	public Sprite simple_wall_deadend_right;
	public Sprite simple_wall_deadend_left;
	[Header("Junctions")]
	public Sprite simple_wall_junction_bottom;
	public Sprite simple_wall_junction_top;
	public Sprite simple_wall_junction_left;
	public Sprite simple_wall_junction_right;
	[Header("Straight")]
	public Sprite simple_wall_horizontal;
	public Sprite simple_wall_vertical;
	[Header("Middle")]
	public Sprite simple_wall_full_middle;
	[Header("Single")]
	public Sprite simple_wall_single;

	[Space(25)]

	[Header("Shop Tileset Sprites")]
	[Header("Floor sprite")]
	public Sprite shop_floor;
	[Header("Corners")]
	public Sprite shop_wall_corner_top_left;
	public Sprite shop_wall_corner_top_right;
	public Sprite shop_wall_corner_bottom_left;
	public Sprite shop_wall_corner_bottom_right;
	[Header("Deadends")]
	public Sprite shop_wall_deadend_bottom;
	public Sprite shop_wall_deadend_top;
	public Sprite shop_wall_deadend_right;
	public Sprite shop_wall_deadend_left;
	[Header("Junctions")]
	public Sprite shop_wall_junction_bottom;
	public Sprite shop_wall_junction_top;
	public Sprite shop_wall_junction_left;
	public Sprite shop_wall_junction_right;
	[Header("Straight")]
	public Sprite shop_wall_horizontal;
	public Sprite shop_wall_vertical;
	[Header("Middle")]
	public Sprite shop_wall_full_middle;
	[Header("Single")]
	public Sprite shop_wall_single;

	void Awake() { instance = this; } 

	public Sprite GetSprite(SpriteType sType) {
		
		switch(sType) {
		case SpriteType.CornerBL:
			if(currentTileSet == TileSet.Simple) return simple_wall_corner_bottom_left;
			else if(currentTileSet == TileSet.Shop) return shop_wall_corner_bottom_left;
			break;
		case SpriteType.CornerBR:
			if(currentTileSet == TileSet.Simple) return simple_wall_corner_bottom_right;
			else if(currentTileSet == TileSet.Shop) return shop_wall_corner_bottom_right;
			break;
		case SpriteType.CornerTL:
			if(currentTileSet == TileSet.Simple) return simple_wall_corner_top_left;
			else if(currentTileSet == TileSet.Shop) return shop_wall_corner_top_left;
			break;
		case SpriteType.CornerTR:
			if(currentTileSet == TileSet.Simple) return simple_wall_corner_top_right;
			else if(currentTileSet == TileSet.Shop) return shop_wall_corner_top_right;
			break;
		case SpriteType.DeadendB:
			if(currentTileSet == TileSet.Simple) return simple_wall_deadend_bottom;
			else if(currentTileSet == TileSet.Shop) return shop_wall_deadend_bottom;
			break;
		case SpriteType.DeadendL:
			if(currentTileSet == TileSet.Simple) return simple_wall_deadend_left;
			else if(currentTileSet == TileSet.Shop) return shop_wall_deadend_left;
			break;
		case SpriteType.DeadendR:
			if(currentTileSet == TileSet.Simple) return simple_wall_deadend_right;
			else if(currentTileSet == TileSet.Shop) return shop_wall_deadend_right;
			break;
		case SpriteType.DeadendT:
			if(currentTileSet == TileSet.Simple) return simple_wall_deadend_top;
			else if(currentTileSet == TileSet.Shop) return shop_wall_deadend_top;
			break;
		case SpriteType.Floor:
			if(currentTileSet == TileSet.Simple) return simple_floor;
			else if(currentTileSet == TileSet.Shop) return shop_floor;
			break;
		case SpriteType.Horizontal:
			if(currentTileSet == TileSet.Simple) return simple_wall_horizontal;
			else if(currentTileSet == TileSet.Shop) return shop_wall_horizontal;
			break;
		case SpriteType.Vertical:
			if(currentTileSet == TileSet.Simple) return simple_wall_vertical;
			else if(currentTileSet == TileSet.Shop) return shop_wall_vertical;
			break;
		case SpriteType.Single:
			if(currentTileSet == TileSet.Simple) return simple_wall_single;
			else if(currentTileSet == TileSet.Shop) return shop_wall_single;
			break;
		case SpriteType.Middle:
			if(currentTileSet == TileSet.Simple) return simple_wall_full_middle;
			else if(currentTileSet == TileSet.Shop) return shop_wall_single;
			break;
		case SpriteType.JunctionB:
			if(currentTileSet == TileSet.Simple) return simple_wall_junction_bottom;
			else if(currentTileSet == TileSet.Shop) return shop_wall_junction_bottom;
			break;
		case SpriteType.JunctionL:
			if(currentTileSet == TileSet.Simple) return simple_wall_junction_left;
			else if(currentTileSet == TileSet.Shop) return shop_wall_junction_left;
			break;
		case SpriteType.JunctionR:
			if(currentTileSet == TileSet.Simple) return simple_wall_junction_right;
			else if(currentTileSet == TileSet.Shop) return shop_wall_junction_right;
			break;
		case SpriteType.JunctionT:
			if(currentTileSet == TileSet.Simple) return simple_wall_junction_top;
			else if(currentTileSet == TileSet.Shop) return shop_wall_junction_top;
			break;
		}

		return null;

	}


}
