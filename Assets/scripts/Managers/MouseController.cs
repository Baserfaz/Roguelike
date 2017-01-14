using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	public static MouseController instance;

	[Header("Cursor textures")]
	public Texture2D cursorTex;

	private Tile chosenTile;
	private GameObject crosshairInst;
	private GameObject playerInst;

	public enum State { CastSpell, UseItem, Normal };
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

						int loop = 0;
						bool finished = false;

						// move automatically to target.
						// uses pathfinding algorithm.
						while(player.position != chosenTile.position) {

							// dont crash unity.
							if(loop > 100){
								Debug.LogError("Loop error: HandleClick");
								break;
							}

							// get the path to target.
							List<Tile> path = new List<Tile>();

							// whether we use breadth-first search or A*.
							if(GameMaster.instance.useAStar) {
								path = player.GetComponent<AStar>().FindPath(chosenTile);

								if(path == null) {
									GUIManager.instance.CreatePopUpEntry("No path", player.position, GUIManager.PopUpType.Other);
									break;
								}

							} else {
								path = player.GetComponent<Pathfinding>().CalculatePathToTarget(chosenTile);
								path.Reverse();
							}

							Tile nextStep = null;

							// get the next step.
							if(GameMaster.instance.useAStar) {

								nextStep = path[0];

							} else {

								try {
									nextStep = path[1];
								} catch {
									GUIManager.instance.CreatePopUpEntry("No path", player.position, GUIManager.PopUpType.Other);
									break;
								}

							}


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

							// increment loop.
							loop ++;

							// if we can see an enemy -> break out of the loop.
							if(finished) break;
						}
					}
				}

			}

		} else if(myState == State.CastSpell) {
			if(Input.GetMouseButtonDown(0)) {
				if(chosenTile == null) return;

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

			} else if(Input.GetMouseButtonDown(1)) {
				
				myState = State.Normal;
				ChangeCrosshairSprite(CrosshairManager.instance.crosshairNormal);

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

	private void Highlight() {
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y), Vector2.zero);

		if(hit) {
			if(hit.transform.GetComponent<Tile>() != null) {
				Tile tile = hit.transform.GetComponent<Tile>();
				if(GameMaster.instance.debugMode) {
					MoveCursor(tile);
				} else {
					if(tile.isVisible || tile.isDiscovered) {
						MoveCursor(tile);
					} else if(tile.isVisible == false && tile.isDiscovered == false) {
						HideCursor();
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
