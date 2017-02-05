using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour {

	public static DungeonGenerator instance;

	private List<GameObject> tiles = new List<GameObject>();
	private GameObject worldParent;

	void Awake() { instance = this; }

	[Header("Dungeon colors")]
	public Color overallTint;

	[Header("Tile prefab")]
	public GameObject tilePrefab;

	public List<GameObject> GetTiles() { return tiles; }
	public GameObject GetWorldParent() { return worldParent; }

	public void DestroyDungeon() {
		foreach(GameObject tile in tiles) {
			Destroy(tile);
		}
		tiles.Clear();

		//Destroy(worldParent);

	}

	public List<TileData> GetAdjacentTileDataAroundPosition(Vector2 pos, bool includeOuterWalls = true) {
		List<TileData> tiles = new List<TileData>();

		GameObject current = null;
		Tile tile = null;
        TileData currentTileData = new TileData();

		// TOP
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y + 1));
		if(current != null) {

            tile = current.GetComponent<Tile>();

            currentTileData.mySpriteType = tile.mySpriteType;

			if(includeOuterWalls) {
                if (tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall || tile.myType == Tile.TileType.DoorClosed)
                {
					currentTileData.mypos = TileData.MYPOS.Top;
					tiles.Add(currentTileData);
				}
			} else {
                if (tile.myType == Tile.TileType.Wall)
                {
					currentTileData.mypos = TileData.MYPOS.Top;
					tiles.Add(currentTileData);
				}
			}
		}

		// LEFT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x - 1, pos.y));
		if(current != null) {

            tile = current.GetComponent<Tile>();

            currentTileData.mySpriteType = tile.mySpriteType;

			if(includeOuterWalls) {
                if (tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall || tile.myType == Tile.TileType.DoorClosed)
                {
					currentTileData.mypos = TileData.MYPOS.Left;
					tiles.Add(currentTileData);
				}
			} else {
                if (tile.myType == Tile.TileType.Wall)
                {
					currentTileData.mypos = TileData.MYPOS.Left;
					tiles.Add(currentTileData);
				}
			}
		}

		// BOTTOM
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y - 1));
		if(current != null) {

            tile = current.GetComponent<Tile>();

            currentTileData.mySpriteType = tile.mySpriteType;

			if(includeOuterWalls) {
                if (tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall || tile.myType == Tile.TileType.DoorClosed)
                {
					currentTileData.mypos = TileData.MYPOS.Bottom;
					tiles.Add(currentTileData);
				}
			} else {
                if (tile.myType == Tile.TileType.Wall)
                {
					currentTileData.mypos = TileData.MYPOS.Bottom;
					tiles.Add(currentTileData);
				}
			}
		}

		// RIGHT
		current = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x + 1, pos.y));
		if(current != null) {

            tile = current.GetComponent<Tile>();

            currentTileData.mySpriteType = tile.mySpriteType;

			if(includeOuterWalls) {
                if (tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall || tile.myType == Tile.TileType.DoorClosed)
                {
					currentTileData.mypos = TileData.MYPOS.Right;
					tiles.Add(currentTileData);
				}
			} else {
                if (tile.myType == Tile.TileType.Wall)
                {
					currentTileData.mypos = TileData.MYPOS.Right;
					tiles.Add(currentTileData);
				}
			}
		}

		return tiles;
	}

	public struct TileData {
		public enum MYPOS { Top, Bottom, Left, Right };
		public MYPOS mypos;
        public SpriteManager.SpriteType mySpriteType;
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

			if(GameMaster.instance.debugMode) {
				
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

	public void UpdateVanityItem(Vector2 pos, GameObject vanityItem) {
		GameObject tileGo = GetTileAtPos(pos);
		Tile tile = tileGo.GetComponent<Tile>();
		tile.vanityItem = vanityItem;
		vanityItem.transform.position = new Vector3(pos.x, pos.y, GameMaster.instance.vanityitemsZLevel);
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

	public int GetNumberOfTilesOfType(Tile.TileType t) {
		int count = 0;
		foreach(GameObject g in tiles) {
			Tile tile = g.GetComponent<Tile>();
			if(tile.GetComponent<Trap>() != null) continue;
			if(tile.myType == t) count++;
		}
		return count;
	}

	public void CreateTile(int x, int y, Tile.TileType type) {
		
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

		} else if(type == Tile.TileType.Trap) {

			GenerateTrap(currentTile);

		}

		currentTile.transform.position = new Vector3(x, y, GameMaster.instance.tileZLevel);
		currentTile.GetComponent<Tile>().position = new Vector2(x, y);

		currentTile.transform.SetParent(worldParent.transform);

		tiles.Add(currentTile);
	}

    private void CalculateSpecialTileSprites()
    {

        // if we should use vertical right or left?
        // if we should use horizontal top or bottom?
        // if we should use vertical both?
        // if we should use horizontal both?

        // junction here.

        foreach (GameObject go in tiles)
        {
            Tile tile = go.GetComponent<Tile>();

            if (tile.myType == Tile.TileType.Wall) //|| tile.myType == Tile.TileType.OuterWall)
            {
                List<TileData> data = GetAdjacentTileDataAroundPosition(tile.position);

                bool isTopWall = false;
                bool isBottomWall = false;
                bool isLeftWall = false;
                bool isRightWall = false;

                TileData top = new TileData();
                TileData bottom = new TileData();
                TileData left = new TileData();
                TileData right = new TileData();

                foreach (TileData d in data)
                {
                    switch (d.mypos)
                    {
                        case TileData.MYPOS.Bottom:
                            isBottomWall = true;
                            bottom = d;
                            break;
                        case TileData.MYPOS.Top:
                            isTopWall = true;
                            top = d;
                            break;
                        case TileData.MYPOS.Left:
                            isLeftWall = true;
                            left = d;
                            break;
                        case TileData.MYPOS.Right:
                            isRightWall = true;
                            right = d;
                            break;
                    }
                }

               // handle outerwall sprites.
               if (tile.myType == Tile.TileType.OuterWall)
               {

                   if (tile.mySpriteType == SpriteManager.SpriteType.CornerTR || tile.mySpriteType == SpriteManager.SpriteType.CornerBR ||
                       tile.mySpriteType == SpriteManager.SpriteType.CornerTL ||tile.mySpriteType == SpriteManager.SpriteType.CornerBL) continue;

                   if (isLeftWall && isBottomWall && !isRightWall && !isTopWall)
                   {
                       tile.mySpriteType = SpriteManager.SpriteType.HorizontalTop;
                       tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalTop);
                   }
                   else if (isLeftWall && isTopWall && !isRightWall && !isBottomWall)
                   {
                       tile.mySpriteType = SpriteManager.SpriteType.HorizontalBottom;
                       tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBottom);
                   }
                   else if (isTopWall && isBottomWall && !isLeftWall && !isRightWall)
                   {
                       tile.mySpriteType = SpriteManager.SpriteType.VerticalBoth;
                       tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalBoth);
                   } 
                   else if(isLeftWall && isRightWall && !isBottomWall && !isTopWall) 
                   {
                       tile.mySpriteType = SpriteManager.SpriteType.HorizontalBoth;
                       tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBoth);
                   }

                   continue;
               }

                // handle wall sprites.
                // vertical both
                if (isTopWall && isBottomWall && !isLeftWall && !isRightWall)
                {
                    if (top.mySpriteType == SpriteManager.SpriteType.DeadendT || bottom.mySpriteType == SpriteManager.SpriteType.DeadendB || 
                        top.mySpriteType == SpriteManager.SpriteType.VerticalBoth || bottom.mySpriteType == SpriteManager.SpriteType.VerticalBoth ||
                        top.mySpriteType == SpriteManager.SpriteType.DoorVerticalClosed || bottom.mySpriteType == SpriteManager.SpriteType.DoorVerticalClosed)
                    {
                        tile.mySpriteType = SpriteManager.SpriteType.VerticalBoth;
                        tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalBoth);
                    }
                }
                    
                else if (isLeftWall && isRightWall && !isTopWall && !isBottomWall)
                {

                    // horizontal both
                    if (left.mySpriteType == SpriteManager.SpriteType.DeadendL || right.mySpriteType == SpriteManager.SpriteType.DeadendR ||
                        left.mySpriteType == SpriteManager.SpriteType.HorizontalBoth || right.mySpriteType == SpriteManager.SpriteType.HorizontalBoth)
                    {
                        tile.mySpriteType = SpriteManager.SpriteType.HorizontalBoth;
                        tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBoth);
                    }
                } 
            }
        }
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

				List<TileData> tilesAround = GetAdjacentTileDataAroundPosition(new Vector2(currentTile.position.x, currentTile.position.y));

				foreach(TileData data in tilesAround) {
					switch(data.mypos) {
					case TileData.MYPOS.Bottom:
						isBottomWall = true;
						break;
					case  TileData.MYPOS.Top:
						isTopWall = true;
						break;
					case TileData.MYPOS.Left:
						isLeftWall = true;
						break;
					case TileData.MYPOS.Right:
						isRightWall = true;
						break;
					}
				}

				if(isTopWall && isBottomWall && isLeftWall && isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.Middle;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.Middle);

				} else if(isTopWall && isLeftWall && isBottomWall && !isRightWall) { 

					//currentTile.mySpriteType = SpriteManager.SpriteType.JunctionL;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionL);

                    currentTile.mySpriteType = SpriteManager.SpriteType.VerticalRight;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalRight);

				} else if(isTopWall && isRightWall && isBottomWall && !isLeftWall) {

				    //currentTile.mySpriteType = SpriteManager.SpriteType.JunctionR;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionR);

                    currentTile.mySpriteType = SpriteManager.SpriteType.VerticalLeft;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalLeft);

				} else if(isLeftWall && isTopWall && isRightWall && !isBottomWall) {

					//currentTile.mySpriteType = SpriteManager.SpriteType.JunctionT;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionT);

                    currentTile.mySpriteType = SpriteManager.SpriteType.HorizontalBottom;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBottom);

				} else if(isLeftWall && isBottomWall && isRightWall && !isTopWall) {

					//currentTile.mySpriteType = SpriteManager.SpriteType.JunctionB;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.JunctionB);

                    currentTile.mySpriteType = SpriteManager.SpriteType.HorizontalTop;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalTop);
                    
				} else if(isTopWall && isBottomWall && !isLeftWall && !isRightWall) {

					//currentTile.mySpriteType = SpriteManager.SpriteType.VerticalLeft;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalLeft);

                    currentTile.mySpriteType = SpriteManager.SpriteType.VerticalBoth;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.VerticalBoth);

                } else if(isLeftWall && isRightWall && !isTopWall && !isBottomWall) {

					//currentTile.mySpriteType = SpriteManager.SpriteType.HorizontalBottom;
					//current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBottom);

                    currentTile.mySpriteType = SpriteManager.SpriteType.HorizontalBoth;
                    current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.HorizontalBoth);

				} else if(isTopWall && isRightWall && !isLeftWall && !isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerBL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerBL);

				} else if(isRightWall && isBottomWall && !isTopWall && !isLeftWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerTL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerTL);

				} else if(isBottomWall && isLeftWall && !isTopWall && !isRightWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerTR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerTR);

				} else if(isLeftWall && isTopWall && !isRightWall && !isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.CornerBR;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.CornerBR);

				} else if(isTopWall && !isBottomWall && !isRightWall && !isLeftWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendB;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendB);

				} else if(isRightWall && !isLeftWall && !isTopWall && !isBottomWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendL;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendL);

				} else if(isBottomWall && !isTopWall && !isRightWall && !isLeftWall) {

					currentTile.mySpriteType = SpriteManager.SpriteType.DeadendT;
					current.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DeadendT);

				} else if(isLeftWall && !isRightWall && !isTopWall && !isBottomWall) {

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

    public void UpdateTileType(Tile tile, Tile.TileType type)
    {

        Vector2 pos = tile.position;

        tiles.Remove(tile.gameObject);
        Destroy(tile.gameObject);

        CreateTile(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), type);
    }

	/// <summary>
	/// Generates a door to a tile.
	/// To get the door/wall sprites right:
	/// 1. generate walls
	/// 2. calculate wall sprites
	/// 3. generate doors from walls.
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void GenerateDoor(GameObject tile) {
		
		tile.AddComponent<Door>();

		Tile current = tile.GetComponent<Tile>();

		TileData[] tds = GetAdjacentTileDataAroundPosition(current.position).ToArray();

		bool isTopWall = false;
		bool isBottomWall = false;
		bool isLeftWall = false;
		bool isRightWall = false;

		foreach(TileData td in tds) {
			switch(td.mypos) {
			case TileData.MYPOS.Bottom:
				isBottomWall = true;
				break;
			case  TileData.MYPOS.Top:
				isTopWall = true;
				break;
			case TileData.MYPOS.Left:
				isLeftWall = true;
				break;
			case TileData.MYPOS.Right:
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
					GenerateDoor(tile);
				}
			}
		}
	}

	/// <summary>
	/// Generates the trap. 
	/// MORE LIKE A CONVERT FLOOR TO TRAP -FUNCTION!
	/// </summary>
	/// <param name="tile">Tile.</param>
	public void GenerateTrap(GameObject tile) {

		// tile is floor tile that will be turned into a trap.

		// add trap component.
		tile.AddComponent<Trap>();

		// actors can walk on it.
		// -> so leave type to floor.
		tile.GetComponent<Tile>().myType = Tile.TileType.Floor;

		// set it's graphics
		tile.GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(
			SpriteManager.SpriteType.TrapOff);
	}

	private void GenerateTraps() {
		if(GameMaster.instance.GenerateTraps) {
			foreach(GameObject go in tiles) {
				Tile current = go.GetComponent<Tile>();
				if(current.myType == Tile.TileType.Floor) {
					if(Random.Range(0, 100) > 100 - GameMaster.instance.trapSpawnChance) {
						GenerateTrap(go);
					}
				}
			}
		}
	}

	/// <summary>
	/// Generate the specified dungeonWidth and dungeonHeight.
	/// Main function.
	/// </summary>
	/// <param name="dungeonWidth">Dungeon width.</param>
	/// <param name="dungeonHeight">Dungeon height.</param>
	public void Generate(int dungeonWidth, int dungeonHeight) {

		// create parent GO.
		if(worldParent == null) {
			worldParent = new GameObject("Dungeon");
			DungeonInfo info = worldParent.AddComponent<DungeonInfo>();
			info.SetHeight(dungeonHeight);
			info.SetWidth(dungeonWidth);
		}


        // We can use different kinds of maze builder scripts here.
        
		//DefaultMaze.Generate(dungeonHeight, dungeonWidth);
        //SimpleMazeBuilder.Generate(dungeonHeight, dungeonWidth);
		//RoomMaze.Generate(dungeonWidth,dungeonHeight);
		CorridorToRoom.Generate(dungeonHeight, dungeonWidth);

		// Modify existing tiles.
		GenerateExit();
		GenerateTraps();
		CalculateWallTileSprites();

		// generate doors only after calculating
		// the wall sprites!

		//GenerateDoors();

		// Create items.
		DungeonItemManager.instance.SpawnItems();
		DungeonVanityManager.instance.SpawnVanityItems();
	}

	public void CreateGrid() {

		if(GameMaster.instance.drawGrid == false) return;

		foreach(GameObject g in tiles) {
			Tile tile = g.GetComponent<Tile>();

			if(tile.myType != Tile.TileType.OuterWall && (tile.isVisible || tile.isDiscovered) && tile.myType == Tile.TileType.Floor) {

				// every even row
				if(tile.position.y % 2 == 0) {
					if(tile.position.x % 2 == 1) {
						tile.GetComponentInChildren<SpriteRenderer>().color = GameMaster.instance.GridTint;
					}
					// every odd row 
				} else {
					if(tile.position.x % 2 == 0) {
						tile.GetComponentInChildren<SpriteRenderer>().color = GameMaster.instance.GridTint;
					}
				}
			}
		}
	}
}
