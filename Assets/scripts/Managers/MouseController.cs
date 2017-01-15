using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    // TODO: 
    // REFACTOR 
    // 1. cast range methods should be in their own class.

	public static MouseController instance;

	[Header("Cursor textures")]
	public Texture2D cursorTex;

	private Tile chosenTile;
	private GameObject crosshairInst;
	private GameObject playerInst;
    private List<Tile> validTiles = null;

	public enum State { CastSpell, UseItem, Normal, OnGui };
	private State myState = State.Normal;

	void Awake() { instance = this; }

	void Start () {
		if(cursorTex != null) Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
	}

	void Update () {
		if(GameMaster.instance.gamestate == GameMaster.GameState.Running && GameMaster.instance.allowMouseInput) {

			if(crosshairInst == null) 
				crosshairInst = (GameObject) Instantiate(CrosshairManager.instance.crosshairPrefab);

			// if the movementmode is set to player
			// we can use mouse input.
			// -> This is because if we are using keyboard input also,
			//    we can be in a "aim-mode". In this mode we don't want
			//    the player to be able to move their character with mouse inputs.
			if(GameMaster.instance.movementMode == GameMaster.MovementMode.Player) {
				if(playerInst == null) playerInst = PrefabManager.instance.GetPlayerInstance();
				Highlight();
				HandleClick();
				Utilities();
			} else {
				HideCursor();
			}
		} else if(GameMaster.instance.gamestate == GameMaster.GameState.Paused) {

			// Handles unpause mechanism.

			if(Input.GetMouseButtonDown(0)) {
				GameMaster.instance.gamestate = GameMaster.GameState.Running;
				PrefabManager.instance.GetPlayerInstance().GetComponent<Player>().inEscMenu = false;
				Destroy(GUIManager.instance.pauseTextGo);
			}

		} else {
			Destroy(crosshairInst);
		}
	}

	public void ChangeCrosshairSprite(Sprite s) { crosshairInst.GetComponent<SpriteRenderer>().sprite = s; }
	public void ChangeState(MouseController.State s) { myState = s; }
    public MouseController.State GetState() { return myState; }

	private void HandleClick() {
		Player player = playerInst.GetComponent<Player>();

		if(myState == State.Normal) {
			if(Input.GetMouseButtonDown(0)) {
				
				// in normal state: (MultiUseKey())
				// 1. move
				// 2. melee
				// 3. loot 
				// 4. open chest
				// 5. buy
				// 6. use ladder (end dungeon)

				if(chosenTile == null) return;

				// pickup
				if(chosenTile.actor != null) {
					if(chosenTile.actor.GetComponent<Player>() != null) {
						MultiUseKey(player);
						return;
					}
				}

				// movement & attack
				// TODO: own system for measuring distance between tiles?
				if(Vector2.Distance(chosenTile.position, player.position) < 1.5f) {
					player.SetMoveTargetPosition(chosenTile.position);
					player.TestAllTileValidities();

					if(player.myNextState == Actor.NextMoveState.Stuck) return;
					else GameMaster.instance.EndTurn();
				} else {

					if(GameMaster.instance.allowPathfinding == false) return;

					if(chosenTile.myType == Tile.TileType.Floor || 
						chosenTile.myType == Tile.TileType.DoorOpen ||
						chosenTile.myType == Tile.TileType.Exit ||
						chosenTile.myType == Tile.TileType.FloorSpecialItem) {

						// if the tile is invisible
						// -> dont move there.
						if(GameMaster.instance.allowPathfindInvisibleTiles == false) {
							if(chosenTile.isVisible == false) return;
						}
						bool finished = false;

						// create a list that stores path.
						List<Tile> path = new List<Tile>();

						// get the path to target.
						path = player.GetComponent<AStar>().FindPath(chosenTile);

						// check if there is no path.
						if(path == null) {
							GUIManager.instance.CreatePopUpEntry("No path", player.position, GUIManager.PopUpType.Other);
							return;
						}

						// reverse path.
						path.Reverse();

						// traverse.
						// because the path is reversed we can
						// use 0 as the start tile.
						for(int i = 0; i < path.Count; i++) {

							// get the next step.
							Tile nextStep = path[i];

							// check if the nextStep is a wall
							// -> trap turns into an unwalkable wall.
							if(nextStep.myType == Tile.TileType.Wall) break;

							// if we can see an enemy
							// cancel out.
							foreach(GameObject g in DungeonGenerator.instance.GetTilesAroundPosition(nextStep.position, 1)) {
								Tile tile = g.GetComponent<Tile>();

								if(tile.actor != null) {
									if(tile.actor.GetComponent<Enemy>() != null) {
										finished = true;
										break;
									}
								}
							}

							// set next move target to be the next step.
							player.SetMoveTargetPosition(nextStep.position);

							// set our state to move.
							player.myNextState = Actor.NextMoveState.Move;

							// end our turn.
							GameMaster.instance.EndTurn();

							// stop moving if there is an enemy nearby.
							if(finished) break;
						}
					}
				}
			}

		} else if(myState == State.CastSpell) {
			if(Input.GetMouseButtonDown(0)) {
				if(chosenTile == null) return;

                // check if we actually clicked on a valid tile.
                // i.e. is on our spell casting range.
                if (validTiles.Contains(chosenTile) == false)
                {
                    GUIManager.instance.CreatePopUpEntry("No range.", playerInst.GetComponent<Player>().position, GUIManager.PopUpType.Other);
                    return;
                }

				// make sure that our next state is NOT move!
				// if it was, our character would move to the
				// same tile as the spell target.
				player.myNextState = Actor.NextMoveState.Pass;

				// update target position
				player.SetMoveTargetPosition(chosenTile.position);

				// cast spell at target position.
				player.HandleCastSpell();

				// normal mode 
				myState = State.Normal;

				// change our crosshair to normal sprite.
				ChangeCrosshairSprite(CrosshairManager.instance.crosshairNormal);

                // Always reset the highlighted area after casting/cancelling.
                ResetHighlighedArea();

			} else if(Input.GetMouseButtonDown(1)) {
				
				myState = State.Normal;
				ChangeCrosshairSprite(CrosshairManager.instance.crosshairNormal);

                // Always reset the highlighted area after casting/cancelling.
                ResetHighlighedArea();
			}
		}
	}

	private void MultiUseKey(Player player) { player.HandleMultiUseKey(); }

	private void Utilities() {
		// zoom
		if(Input.mouseScrollDelta.y > 0f) {
			CameraManager.instance.ZoomIn();
		} else if(Input.mouseScrollDelta.y < 0f) {
			CameraManager.instance.ZoomOut();
		}
	}

	private void HideCursor() {
		chosenTile = null;
		crosshairInst.GetComponent<SpriteRenderer>().color = Color.clear;
	}

	private void ShowCursor() { crosshairInst.GetComponent<SpriteRenderer>().color = Color.white; }

	private void MoveCursor(Tile tile) {
		ShowCursor();
		chosenTile = tile;
		crosshairInst.transform.position = tile.position;
		GUIManager.instance.ShowTileInfo(tile);
	}

    private void ResetHighlighedArea()
    {
        // reset highlighted area.
        foreach (GameObject g in DungeonGenerator.instance.GetTiles())
        {
            Tile t = g.GetComponent<Tile>();
            if (t.isVisible) g.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        }
    }

    private List<Tile> GetSpellRange(Spell spell, Player player)
    {
        List<Tile> range = new List<Tile>();
        Tile current = null;
        GameObject go = null;

        if (spell.isLinear)
        {
            for (int x = 0; x < GameMaster.instance.playerCastRange; x++)
            {
                go = DungeonGenerator.instance.GetTileAtPos(player.position + new Vector2(x, 0f));
                if (go == null) continue;
                current = go.GetComponent<Tile>();
                range.Add(current);
            }

            for (int x = 0; x < GameMaster.instance.playerCastRange; x++)
            {
                go = DungeonGenerator.instance.GetTileAtPos(player.position - new Vector2(x, 0f));
                if (go == null) continue;
                current = go.GetComponent<Tile>();
                range.Add(current);
            }

            for (int y = 0; y < GameMaster.instance.playerCastRange; y++)
            {
                go = DungeonGenerator.instance.GetTileAtPos(player.position + new Vector2(0f, y));
                if (go == null) continue;
                current = go.GetComponent<Tile>();
                range.Add(current);
            }

            for (int y = 0; y < GameMaster.instance.playerCastRange; y++)
            {
                go = DungeonGenerator.instance.GetTileAtPos(player.position - new Vector2(0f, y));
                if (go == null) continue;
                current = go.GetComponent<Tile>();
                range.Add(current);
            }
        }
        else
        {
            foreach (GameObject g in DungeonGenerator.instance.GetTilesAroundPosition(player.position, GameMaster.instance.playerCastRange))
            {
                Tile tile = g.GetComponent<Tile>();
                if (tile.isVisible) range.Add(tile);
            }
        }

        return range;
    }

    private void DrawSpellRange(List<Tile> range)
    {
        foreach (Tile t in range)
        {
            // out of bounds.
            if (t == null) continue;

            if (t.isVisible)
            {
                t.GetComponentInChildren<SpriteRenderer>().color = Color.green;
            }
        }
    }

	private void Highlight() {
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y), Vector2.zero);

        Spell currentSpell = null;

        // draw cast area.
        if (myState == State.CastSpell)
        {
            currentSpell = playerInst.GetComponent<Inventory>().currentSpell.GetComponent<Spell>();

            // reset highlighted area.
            ResetHighlighedArea();

            // get the tiles that are valid.
            validTiles = GetSpellRange(currentSpell, playerInst.GetComponent<Player>());

            // highlight those tiles.
            DrawSpellRange(validTiles);
        }

		if(hit) {
			if(hit.transform.GetComponent<Tile>() != null) {
				Tile tile = hit.transform.GetComponent<Tile>();
				if(GameMaster.instance.debugMode) {
					MoveCursor(tile);
				} else {

                    if (myState == State.CastSpell)
                    {

                        MoveCursor(tile);

                        if (tile.isVisible)
                        {

                            Spell.DamageInfo di = new Spell.DamageInfo();
                            di.targetTile = tile;
                            di.damageDealer = playerInst;

                            GameObject[] aoe = currentSpell.CalculateAOE(di);

                            // if there is no area, return.
                            if (aoe == null) return;

                            // if we are not hovering on top of a valid tile, then return.
                            if (validTiles.Contains(di.targetTile) == false) return;

                            // --> otherwise draw the area.

                            // set new highlight area of the spell!
                            foreach (GameObject g in aoe)
                            {
                                // out of bounds.
                                if (g == null) continue;

                                Tile _tile = g.GetComponent<Tile>();
                                if (_tile.isVisible)
                                {
                                    g.GetComponentInChildren<SpriteRenderer>().color = Color.red;
                                }
                            }
                        }
                    }
                    else if (myState == State.Normal)
                    {
                        if (tile.isVisible || tile.isDiscovered)
                        {
                            MoveCursor(tile);
                        }
                        else if (tile.isVisible == false && tile.isDiscovered == false)
                        {
                            HideCursor();
                        }
                    }

				}
			} else {
				HideCursor();
			}
		} else {
			HideCursor();
		}
	}
}
