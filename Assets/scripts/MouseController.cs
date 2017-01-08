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
		if(GameMaster.instance.isGameRunning) {

			if(crosshairInst == null) 
				crosshairInst = (GameObject) Instantiate(CrosshairManager.instance.crosshairPrefab);

			if(GameMaster.instance.movementMode == GameMaster.MovementMode.Player) {
				if(playerInst == null) playerInst = PrefabManager.instance.GetPlayerInstance();
				Highlight();
				HandleClick();
				Utilities();
			} else {
				HideCursor();
			}
		} else {
			Destroy(crosshairInst);
		}
	}

	public void ChangeCrosshairSprite(Sprite s) {
		crosshairInst.GetComponent<SpriteRenderer>().sprite = s;
	}

	public void ChangeState(MouseController.State s) {
		myState = s;
	}

	private void HandleClick() {
		Player player = playerInst.GetComponent<Player>();

		if(myState == State.Normal) {
			if(Input.GetMouseButtonDown(0)) {

				// in normal state:
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
				if(Vector2.Distance(chosenTile.position, player.position) < 1.5f) {
					player.SetMoveTargetPosition(chosenTile.position);
					player.TestAllTileValidities();

					if(player.myNextState == Actor.NextMoveState.Stuck) return;
					else GameMaster.instance.EndTurn();
				}

			}

		} else if(myState == State.CastSpell) {
			if(Input.GetMouseButtonDown(0)) {
				if(chosenTile == null) return;

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

	private void MultiUseKey(Player player) {
		player.HandleMultiUseKey();
	}

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

	private void ShowCursor() {
		crosshairInst.GetComponent<SpriteRenderer>().color = Color.white;
	}

	private void Highlight() {
		Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(new Vector2(pos.x, pos.y), Vector2.zero);

		if(hit) {
			if(hit.transform.GetComponent<Tile>() != null) {
				Tile tile = hit.transform.GetComponent<Tile>();
				if(tile.isVisible || tile.isDiscovered) {
					ShowCursor();
					chosenTile = tile;
					crosshairInst.transform.position = tile.position;
				} else if(tile.isVisible == false && tile.isDiscovered == false) {
					HideCursor();
				}
			} else {
				HideCursor();
			}
		} else {
			HideCursor();
		}

	}

}
