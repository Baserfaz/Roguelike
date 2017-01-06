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

	[Header("Tile prefab")]
	public GameObject tilePrefab;

	public List<GameObject> GetTiles() { return tiles; }

	public void DestroyDungeon() {
		foreach(GameObject tile in tiles) {
			Destroy(tile);
		}
		tiles.Clear();
	}

	public List<tileData> GetAdjacentTileDataAroundPosition(Vector2 pos, bool includeOuterWalls = true) {
		List<tileData> tiles = new List<tileData>();

		GameObject current = null;
		tileData currentTileData = new tileData();

		// TOP
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y + 1));
		if(current != null) {

			if(includeOuterWalls) {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
					currentTileData.mypos = tileData.MYPOS.Top;
					tiles.Add(currentTileData);
				}
			} else {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall) {
					currentTileData.mypos = tileData.MYPOS.Top;
					tiles.Add(currentTileData);
				}
			}
		}

		// LEFT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x - 1, pos.y));
		if(current != null) {

			if(includeOuterWalls) {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
					currentTileData.mypos = tileData.MYPOS.Left;
					tiles.Add(currentTileData);
				}
			} else {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall) {
					currentTileData.mypos = tileData.MYPOS.Left;
					tiles.Add(currentTileData);
				}
			}
		}

		// BOTTOM
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y - 1));
		if(current != null) {

			if(includeOuterWalls) {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
					currentTileData.mypos = tileData.MYPOS.Bottom;
					tiles.Add(currentTileData);
				}
			} else {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall) {
					currentTileData.mypos = tileData.MYPOS.Bottom;
					tiles.Add(currentTileData);
				}
			}
		}

		// RIGHT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x + 1, pos.y));
		if(current != null) {

			if(includeOuterWalls) {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall || current.GetComponent<Tile>().myType == Tile.TileType.OuterWall) {
					currentTileData.mypos = tileData.MYPOS.Right;
					tiles.Add(currentTileData);
				}
			} else {
				if(current.GetComponent<Tile>().myType == Tile.TileType.Wall) {
					currentTileData.mypos = tileData.MYPOS.Right;
					tiles.Add(currentTileData);
				}
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
				if(t.vanityItem != null) t.vanityItem.GetComponent<VanityItem>().ShowItem();

			} else {
				
				if(t.isVisible && t.isDiscovered) {
					t.Show();
					if(t.actor != null) t.actor.GetComponent<Actor>().Show();
					if(t.item != null) t.item.GetComponent<Item>().ShowItem();
					if(t.vanityItem != null) t.vanityItem.GetComponent<VanityItem>().ShowItem();
					continue;
				}

				if(t.isDiscovered) {
					t.ShowAsDiscovered();
					if(t.actor != null) t.actor.GetComponent<Actor>().Hide();
					if(t.item != null) t.item.GetComponent<Item>().HideItem();
					if(t.vanityItem != null) t.vanityItem.GetComponent<VanityItem>().HideItem();
					continue;
				}

				if(t.isVisible == false) { 
					t.Hide();
					if(t.actor != null) t.actor.GetComponent<Actor>().Hide();
					if(t.item != null) t.item.GetComponent<Item>().HideItem();
					if(t.vanityItem != null) t.vanityItem.GetComponent<VanityItem>().HideItem();
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
			
			currentTile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Floor);
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Floor;

		} else if(type == Tile.TileType.Wall) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Wall;

		} else if(type == Tile.TileType.Exit) {

			currentTile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Exit);
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Exit;

		} else if(type == Tile.TileType.OuterWall) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.OuterWall;

		} else if(type == Tile.TileType.FloorSpecialItem) {
			
			currentTile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.FloorSpecialItem);
			currentTile.GetComponent<Tile>().myType = Tile.TileType.Floor;

		} else if(type == Tile.TileType.DoorClosed) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.DoorClosed;

		} else if(type == Tile.TileType.DoorOpen) {
			
			currentTile.GetComponent<Tile>().myType = Tile.TileType.DoorOpen;
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

					currentTile.mySpriteType = SpriteManager.SpriteType.Middle;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Middle);

				} else if(isTopWall && isLeftWall && isBottomWall) { 

					currentTile.mySpriteType = SpriteManager.SpriteType.JunctionL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionL);

				} else if(isTopWall && isRightWall && isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.JunctionR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionR);

				} else if(isLeftWall && isTopWall && isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.JunctionT;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionT);

				} else if(isLeftWall && isBottomWall && isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.JunctionB;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionB);

				} else if(isTopWall && isBottomWall) {
					
					currentTile.mySpriteType = SpriteManager.SpriteType.Vertical;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Vertical);

				} else if(isLeftWall && isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.Horizontal;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Horizontal);
		
				} else if(isTopWall && isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerBL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerBL);

				} else if(isRightWall && isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerTL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerTL);

				} else if(isBottomWall && isLeftWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerTR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerTR);

				} else if(isLeftWall && isTopWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerBR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerBR);

				} else if(isTopWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendB;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendB);

				} else if(isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendL);

				} else if(isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendT;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendT);

				} else if(isLeftWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendR);

				} else {

					currentTile.mySpriteType = SpriteManager.SpriteType.Single;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Single);

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

	/// <summary>
	/// Generates a door to a tile.
	/// To get the door/wall sprites right:
	/// 1. generate walls
	/// 2. calculate wall sprites
	/// 3. generate doors from walls.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void CalculateDoorSprite(GameObject tile) {
		Tile current = tile.GetComponent<Tile>();

		tileData[] tds = GetAdjacentTileDataAroundPosition(current.position).ToArray();

		bool isTopWall = false;
		bool isBottomWall = false;
		bool isLeftWall = false;
		bool isRightWall = false;

		foreach(tileData td in tds) {
			switch(td.mypos) {
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

		if(isTopWall && isBottomWall && isLeftWall == false && isRightWall == false) {
			tile.GetComponent<Tile>().myType = Tile.TileType.DoorClosed;
			tile.GetComponent<Tile>().mySpriteType = SpriteManager.SpriteType.DoorVerticalClosed;
			tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorVerticalClosed);
		} else if(isLeftWall && isRightWall && isTopWall == false && isBottomWall == false) {
			tile.GetComponent<Tile>().myType = Tile.TileType.DoorClosed;
			tile.GetComponent<Tile>().mySpriteType = SpriteManager.SpriteType.DoorHorizontalClosed;
			tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorHorizontalClosed);
		}
	}

	/// <summary>
	/// Generates doors from random walls.
	/// To get the door/wall sprites right:
	/// 1. generate walls
	/// 2. calculate wall sprites
	/// 3. generate doors from walls.
	/// </summary>
	private void GenerateDoors() {

		foreach(GameObject tile in tiles) {
			Tile current = tile.GetComponent<Tile>();

			if(current.myType == Tile.TileType.Wall) {

				// only sometimes create a door.
				if(Random.Range(0, 100) > 100 - GameMaster.instance.doorSpawnChance) {
					CalculateDoorSprite(tile);
				}
			}
		}
	}

	public void Generate(int dungeonWidth, int dungeonHeight) {

		for(int y = 0; y < dungeonHeight; y++) {
			for(int x = 0; x < dungeonWidth; x++) {

				// outer walls.
				if(y == 0 || y == dungeonHeight - 1 || x == 0 || x == dungeonWidth - 1) {
					CreateTile(x, y, Tile.TileType.OuterWall);
					continue;
				}
					
				// create walls.
				int rand = Random.Range(1, GameMaster.instance.dungeonSpaciousness);
				if(x % rand == 0 || y % rand == 0) {
					CreateTile(x, y, Tile.TileType.Wall);
					continue;
				}

				// create floor.
				CreateTile(x, y, Tile.TileType.Floor);
			}
		}

		GenerateExit();
		CalculateWallTileSprites();
		GenerateDoors();
		DungeonVanityManager.instance.SpawnVanityItems();
	}
}
