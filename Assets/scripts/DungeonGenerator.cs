using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

	public static DungeonGenerator instance;

	[Header("Debug settings")]
	public bool debugMode = true;

	private List<GameObject> tiles = new List<GameObject>();
	private GameObject worldParent;

	void Awake() { instance = this; }

	[Header("Dungeon colors")]
	public Color overallTint;

	[Header("floor sprite")]
	public Sprite s_floor;

	[Header("Corners")]
	public Sprite s_wall_corner_top_left;
	public Sprite s_wall_corner_top_right;
	public Sprite s_wall_corner_bottom_left;
	public Sprite s_wall_corner_bottom_right;

	[Header("Deadends")]
	public Sprite s_wall_deadend_bottom;
	public Sprite s_wall_deadend_top;
	public Sprite s_wall_deadend_right;
	public Sprite s_wall_deadend_left;

	[Header("Junctions")]
	public Sprite s_wall_junction_bottom;
	public Sprite s_wall_junction_top;
	public Sprite s_wall_junction_left;
	public Sprite s_wall_junction_right;

	[Header("Straight")]
	public Sprite s_wall_horizontal;
	public Sprite s_wall_vertical;

	[Header("Middle")]
	public Sprite s_wall_full_middle;

	[Header("Single")]
	public Sprite s_wall_single;

	[Header("Other")]
	public Sprite s_exit;

	[Header("Tile prefabs")]
	public GameObject tilePrefab;

	public List<GameObject> GetTiles() { return tiles; }

	public void DestroyDungeon() {
		foreach(GameObject tile in tiles) {
			Destroy(tile);
		}
		tiles.Clear();
	}

	public List<tileData> GetAdjacentTileDataAroundPosition(Vector2 pos) {
		List<tileData> tiles = new List<tileData>();

		GameObject current = null;
		tileData currentTileData = new tileData();

		// TOP
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y + 1));
		if(current != null) {
			if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
				currentTileData.mypos = tileData.MYPOS.Top;
		 		tiles.Add(currentTileData);
			}
		}

		// LEFT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x - 1, pos.y));
		if(current != null) {
			if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
				currentTileData.mypos = tileData.MYPOS.Left;
				tiles.Add(currentTileData);
			}
		}

		// BOTTOM
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y - 1));
		if(current != null) {
			if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
				currentTileData.mypos = tileData.MYPOS.Bottom;
				tiles.Add(currentTileData);
			}
		}

		// RIGHT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x + 1, pos.y));
		if(current != null) {
			if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
				currentTileData.mypos = tileData.MYPOS.Right;
				tiles.Add(currentTileData);
			}
		}

		return tiles;
	}

	public struct tileData {
		public enum MYPOS { Top, Bottom, Left, Right };
		public MYPOS mypos;
	}

	public GameObject GetFirstFreeTileNearPosition(Vector2 pos) {
		for(int y = -1; y < 2; y++) {
			for(int x = -1; x < 2; x++) {
				GameObject current = DungeonGenerator.instance.GetTileAtPos(new Vector2(x, y) + pos);
				if(current == null) continue;

				Tile tile = current.GetComponent<Tile>();

				if(tile.myType == Tile.TileType.Floor && tile.actor == null && tile.item == null) {
					return current;
				}
			}
		}

		return null;
	}

	public List<GameObject> GetTilesAroundPosition(Vector2 pos, int radius) {
		List<GameObject> tiles = new List<GameObject>();

		for(int y = -1 * radius; y < 2 + radius - 1; y++) {
			for(int x = -1 * radius; x < 2 + radius - 1; x++) {
				GameObject current = DungeonGenerator.instance.GetTileAtPos(new Vector2(x, y) + pos);
				if(current == null) continue;
				tiles.Add(current);
			}
		}
		return tiles;
	}

	public void UpdateTileColorVisibility() {
		foreach(GameObject tile in tiles) {
			Tile t = tile.GetComponent<Tile>();

			if(debugMode) {
				
				t.Show();
				if(t.actor != null) t.actor.GetComponent<Actor>().Show();
				if(t.item != null) t.item.GetComponent<Item>().ShowItem();

			} else {
				
				if(t.isVisible && t.isDiscovered) {
					t.Show();
					if(t.actor != null) t.actor.GetComponent<Actor>().Show();
					if(t.item != null) t.item.GetComponent<Item>().ShowItem();
					continue;
				}

				if(t.isDiscovered) {
					t.ShowAsDiscovered();
					if(t.actor != null) t.actor.GetComponent<Actor>().Hide();
					if(t.item != null) t.item.GetComponent<Item>().HideItem();
					continue;
				}

				if(t.isVisible == false) { 
					t.Hide();
					if(t.actor != null) t.actor.GetComponent<Actor>().Hide();
					if(t.item != null) t.item.GetComponent<Item>().HideItem();
				}

			}
		}
	}

	public void UpdateTileActor(Vector2 pos, GameObject actor) {
		GameObject tileGo = GetTileAtPos(pos);
		Tile tile = tileGo.GetComponent<Tile>();
		tile.actor = actor;
	}

	public void UpdateTileItem(Vector2 pos, GameObject item) {
		GameObject tileGo = GetTileAtPos(pos);
		Tile tile = tileGo.GetComponent<Tile>();
		tile.item = item;
	}

	public GameObject GetTileAtPos(Vector2 pos) {
		GameObject retObj = null;
		for(int i = 0; i < tiles.Count; i++) {
			GameObject current = tiles[i];
			Tile tile = current.GetComponent<Tile>();
			if(Mathf.FloorToInt(tile.position.x) == Mathf.FloorToInt(pos.x) && Mathf.FloorToInt(tile.position.y) == Mathf.FloorToInt(pos.y)) {
				retObj = current;
				break;
			}
		}
		return retObj;
	}

	public void CreateTile(int x, int y, Tile.TileType type) {

		// create parent GO.
		if(worldParent == null) worldParent = new GameObject("Dungeon");

		GameObject currentTile = (GameObject) Instantiate(tilePrefab);

		if(type == Tile.TileType.Floor) {
			
			currentTile.GetComponentInChildren<SpriteRenderer>().sprite = s_floor;
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Floor;

		} else if(type == Tile.TileType.Wall) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Wall;

		} else if(type == Tile.TileType.Exit) {

			currentTile.GetComponentInChildren<SpriteRenderer>().sprite = s_exit;
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Exit;

		} else if(type == Tile.TileType.OuterWall) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.OuterWall;
		}

		currentTile.transform.position = new Vector3(x, y, GameMaster.instance.tileZLevel);
		currentTile.GetComponent<Tile>().position = new Vector2(x, y);

		currentTile.transform.SetParent(worldParent.transform);

		tiles.Add(currentTile);
	}

	public void CalculateWallTileSprites() {
		
		for(int i = 0; i < tiles.Count; i++) {

			bool isTopWall = false;
			bool isBottomWall = false;
			bool isLeftWall = false;
			bool isRightWall = false;

			GameObject current = tiles[i];
			Tile currentTile = current.GetComponent<Tile>();

			if(currentTile.myType == Tile.TileType.Wall || currentTile.myType == Tile.TileType.OuterWall) {

				List<tileData> tilesAround = GetAdjacentTileDataAroundPosition(new Vector2(currentTile.position.x, currentTile.position.y));

				foreach(tileData data in tilesAround) {
					switch(data.mypos) {
					case tileData.MYPOS.Bottom:
						isBottomWall = true;
						break;
					case  tileData.MYPOS.Top:
						isTopWall = true;
						break;
					case tileData.MYPOS.Left:
						isLeftWall = true;
						break;
					case tileData.MYPOS.Right:
						isRightWall = true;
						break;
					}
				}

				if(isTopWall && isBottomWall && isLeftWall && isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_full_middle;

				} else if(isTopWall && isLeftWall && isBottomWall) { 
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_junction_left;

				} else if(isTopWall && isRightWall && isBottomWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_junction_right;

				} else if(isLeftWall && isTopWall && isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_junction_top;

				} else if(isLeftWall && isBottomWall && isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_junction_bottom;

				} else if(isTopWall && isBottomWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_vertical;

				} else if(isLeftWall && isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_horizontal;

				} else if(isTopWall && isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_corner_bottom_left;

				} else if(isRightWall && isBottomWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_corner_top_left;

				} else if(isBottomWall && isLeftWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_corner_top_right;

				} else if(isLeftWall && isTopWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_corner_bottom_right;

				} else if(isTopWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_deadend_bottom;

				} else if(isRightWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_deadend_left;

				} else if(isBottomWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_deadend_top;

				} else if(isLeftWall) {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_deadend_right;

				} else {
					
					current.GetComponentInChildren<SpriteRenderer>().sprite = s_wall_single;

				}
			}

			currentTile.SetStartColor(overallTint);
		}
	}

	public void GenerateExit() {
		List<GameObject> candidates = new List<GameObject>();

		foreach(GameObject tileGo in tiles) {
			Tile tile = tileGo.GetComponent<Tile>();
			if(tile.myType == Tile.TileType.Floor) {
				candidates.Add(tileGo);
			}
		}

		GameObject candidate = candidates[Random.Range(0, candidates.Count)];
		Vector2 position = candidate.GetComponent<Tile>().position;

		tiles.Remove(candidate);
		Destroy(candidate);

		CreateTile(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Tile.TileType.Exit);
	}

	public void Generate(int dungeonWidth, int dungeonHeight) {

		for(int y = 0; y < dungeonHeight; y++) {
			for(int x = 0; x < dungeonWidth; x++) {

				// outer walls
				if(y == 0 || y == dungeonHeight - 1 || x == 0 || x == dungeonWidth - 1) {
					CreateTile(x, y, Tile.TileType.OuterWall);
					continue;
				}
					
				int rand = Random.Range(1, GameMaster.instance.dungeonSpaciousness);

				if(x % rand == 0 || y % rand == 0) {
					CreateTile(x, y, Tile.TileType.Wall);
					continue;
				}

				CreateTile(x, y, Tile.TileType.Floor);
			}
		}

		GenerateExit();
		CalculateWallTileSprites();
	}
}
