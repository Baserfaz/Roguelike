using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor {

	public int seeRadius = 1;

	private void Update() {
		ManageInputs();
	}

	// Checks if the tile is walkable.
	private void TestTileValidity() {

		GameObject targetTile = DungeonGenerator.instance.GetTileAtPos(moveTargetPosition);
		Tile tile = targetTile.GetComponent<Tile>();

		if(tile.myType == Tile.TileType.Floor || tile.myType == Tile.TileType.Exit || tile.myType == Tile.TileType.DoorOpen) {
			myNextState = NextMoveState.Move;
		} else if(tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall || tile.myType == Tile.TileType.DoorClosed) {
			
			// when player moves toward door it opens and player their passes turn.

			if(tile.myType == Tile.TileType.DoorClosed) {
				tile.GetComponent<Door>().OpenDoor();
				myNextState = NextMoveState.Pass;
			} else {
				myNextState = NextMoveState.Stuck;
			}
		}
	}

	// Checks if there is an actor
	// in the tile we are trying to move to.
	private void TestTileActorValidity() {
		GameObject targetTile = DungeonGenerator.instance.GetTileAtPos(moveTargetPosition);
		Tile tile = targetTile.GetComponent<Tile>();

		if(tile.actor == null) {
			myNextState = NextMoveState.None;
		} else {
			myNextState = NextMoveState.Attack;
		}
	}

	private void TestAllTileValidities() {

		// first check if the next tile 
		// has an actor in it. -> attack etc.
		// else test if we can move there.
		TestTileActorValidity();

		// actor test will update nextmovestate to pass
		// if the next tile is empty.

		if(myNextState == NextMoveState.None) TestTileValidity();
	}

	private void Movement() {
		bool pressed = false;

		myNextState = NextMoveState.None;

		// movement
		if(Input.GetKeyDown(KeyCode.Keypad7)) {
			moveTargetPosition = new Vector2(position.x - 1f, position.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad8)) {
			moveTargetPosition = new Vector2(position.x, position.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad9)) {
			moveTargetPosition = new Vector2(position.x + 1f, position.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad4)) {
			moveTargetPosition = new Vector2(position.x - 1f, position.y);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad6)) {
			moveTargetPosition = new Vector2(position.x + 1f, position.y);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad1)) {
			moveTargetPosition = new Vector2(position.x - 1f, position.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad2)) {
			moveTargetPosition = new Vector2(position.x, position.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad3)) {
			moveTargetPosition = new Vector2(position.x + 1f, position.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad5)) {

			// keypad5 is the general key for (in this order)
			// 1. picking up items
			// 2. exiting level
			// 3. passing turn

			Tile tile = DungeonGenerator.instance.GetTileAtPos(position).GetComponent<Tile>();

			if(tile.item != null) {

				if(tile.item.GetComponent<Item>().myState == Item.State.Free) {
					
					// pick the item up

					PickUpItem(tile.item);
					GameMaster.instance.EndTurn();

				} else if(tile.item.GetComponent<Item>().myState == Item.State.Shop) {
					
					Item item = tile.item.GetComponent<Item>();

					// buy item.
					if(GetComponent<Inventory>().currentGold >= item.shopPrice) {
				
						// destroy guitext.
						Destroy(GUIManager.instance.currentActiveShopGo);

						GetComponent<Inventory>().currentGold -= item.shopPrice;
						item.myState = Item.State.Free;
						PickUpItem(tile.item);

					} else {

						// not enough money.
						GUIManager.instance.CreatePopUpEntry("Not enough money", position, GUIManager.PopUpType.Other);
						GUIManager.instance.CreateJournalEntry("Not enough money to buy " + item.itemName, GUIManager.JournalType.Item);

					}
				}

			} else if(tile.myType == Tile.TileType.Exit) {
				GameMaster.instance.ExitDungeon();
			} else {
				GameMaster.instance.EndTurn();
			}
		} else if(Input.GetKeyDown(KeyCode.Keypad0)) {
			// use item

			if(GetComponent<Inventory>().currentUseItem != null) {
				GameObject item = GetComponent<Inventory>().currentUseItem;

				GUIManager.instance.CreateJournalEntry("Used item " + "[" + item.GetComponent<Item>().itemName + "]", GUIManager.JournalType.Item);

				if(item.GetComponent<Potion>() != null) {
					item.GetComponent<Potion>().Drink();
				}

				GameMaster.instance.EndTurn();

			} else {
				GUIManager.instance.CreateJournalEntry("You don't have any item to use.", GUIManager.JournalType.Item);
			}

		} else if(Input.GetKeyDown(KeyCode.KeypadEnter)) {

			// cast spell
			// --> crosshair state.
			if(GetComponent<Inventory>().currentSpell != null) {

				// choose tile by using crosshair.
				// --> uses CrosshairMovement()
				CrosshairManager.instance.CrosshairMode(position);

			} else {
				GUIManager.instance.CreateJournalEntry("You don't have a spell.", GUIManager.JournalType.Item);
			}
		}

		// for movement.
		if(pressed) { 

			TestAllTileValidities();

			if(myNextState == NextMoveState.Stuck) return;
			else GameMaster.instance.EndTurn();
		}
	}

	private void CrosshairMovement() {

		bool pressed = false;

		Vector2 crosshairPos = CrosshairManager.instance.GetCrosshairInstance().GetComponent<Crosshair>().position;

		// crosshairmovement
		if(Input.GetKeyDown(KeyCode.Keypad7)) {
			moveTargetPosition = new Vector2(crosshairPos.x - 1f, crosshairPos.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad8)) {
			moveTargetPosition = new Vector2(crosshairPos.x, crosshairPos.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad9)) {
			moveTargetPosition = new Vector2(crosshairPos.x + 1f, crosshairPos.y + 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad4)) {
			moveTargetPosition = new Vector2(crosshairPos.x - 1f, crosshairPos.y);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad6)) {
			moveTargetPosition = new Vector2(crosshairPos.x + 1f, crosshairPos.y);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad1)) {
			moveTargetPosition = new Vector2(crosshairPos.x - 1f, crosshairPos.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad2)) {
			moveTargetPosition = new Vector2(crosshairPos.x, crosshairPos.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.Keypad3)) {
			moveTargetPosition = new Vector2(crosshairPos.x + 1f, crosshairPos.y - 1);
			pressed = true;
		} else if(Input.GetKeyDown(KeyCode.KeypadEnter)) {

			// cast spell here.

			// if we can see the targeted tile.
			if(DungeonGenerator.instance.GetTileAtPos(moveTargetPosition).GetComponent<Tile>().isVisible) {

				GameObject spell = GetComponent<Inventory>().currentSpell;

				if(spell.GetComponent<Spell>().currentCooldown == 0) {

					spell.GetComponent<Spell>().Cast(moveTargetPosition);

					GUIManager.instance.CreatePopUpEntry(spell.GetComponent<Item>().itemName, position, GUIManager.PopUpType.Other);
					GUIManager.instance.CreateJournalEntry("Cast spell " + "[" + spell.GetComponent<Item>().itemName + "]", GUIManager.JournalType.Item);

				} else {
					GUIManager.instance.CreatePopUpEntry("COOLDOWN", position, GUIManager.PopUpType.Other);
					GUIManager.instance.CreateJournalEntry("Can't cast yet.", GUIManager.JournalType.Combat);

					CrosshairManager.instance.PlayerMode();
					return;
				}

				CrosshairManager.instance.PlayerMode();
				GameMaster.instance.EndTurn();

			} else {

				GUIManager.instance.CreatePopUpEntry("Can't see target", position, GUIManager.PopUpType.Other);
				GUIManager.instance.CreateJournalEntry("Can't see target.", GUIManager.JournalType.Combat);
				CrosshairManager.instance.PlayerMode();
			}

		} else if(Input.GetKeyDown(KeyCode.Keypad5)) {
			// cancel cast.
			// -> return to player mode.
			CrosshairManager.instance.PlayerMode();
		}

		if(pressed) {
			CrosshairManager.instance.GetCrosshairInstance().GetComponent<Crosshair>().MoveCrosshair(moveTargetPosition);
		}

	}

	private void PickUpItem(GameObject itemGo) {

		// 0.
		Inventory inventory = GetComponent<Inventory>();
		Item item = itemGo.GetComponent<Item>();

		// 1.
		inventory.HandleItem(itemGo);

		// 2.
		if(itemGo.GetComponent<Gold>() == null) {
			GUIManager.instance.CreateJournalEntry("Picked up " + "[" + item.itemName + "]", GUIManager.JournalType.Item);
			GUIManager.instance.CreatePopUpEntry(item.itemName + "\n" + "<size=\"17\">" + item.itemDescription + "</size>", position, GUIManager.PopUpType.Other, 3f);
		}
	}

	private void Utilities() {
		// zoom
		if(Input.GetKey(KeyCode.KeypadPlus)) {
			CameraManager.instance.ZoomIn();
		} else if(Input.GetKey(KeyCode.KeypadMinus)) {
			CameraManager.instance.ZoomOut();
		}
	}

	private void ManageInputs() {
		
		if(GameMaster.instance.movementMode == GameMaster.MovementMode.Player) {
			Movement();
		} else {
			CrosshairMovement();
		}
			
		Utilities();
	}
}
