﻿using System.Collections;
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

		if(tile.myType == Tile.TileType.Floor || tile.myType == Tile.TileType.Exit) {
			myNextState = NextMoveState.Move;
		} else if(tile.myType == Tile.TileType.Wall || tile.myType == Tile.TileType.OuterWall) {
			myNextState = NextMoveState.Stuck;
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
				PickUpItem(tile.item);
				GameMaster.instance.EndTurn();
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

				GUIManager.instance.UpdateAllElements();

				GameMaster.instance.EndTurn();

			} else {
				GUIManager.instance.CreateJournalEntry("You don't have any item to use.", GUIManager.JournalType.Item);
			}

		}

		// for movement.
		if(pressed) { 

			TestAllTileValidities();

			if(myNextState == NextMoveState.Stuck) return;
			else GameMaster.instance.EndTurn();
		}
	}

	private void PickUpItem(GameObject itemGo) {

		// 0.
		Inventory inventory = GetComponent<Inventory>();
		Item item = itemGo.GetComponent<Item>();

		// 1.
		inventory.HandleItem(itemGo);

		// 2.
		GUIManager.instance.CreateJournalEntry("Picked up " + "[" + item.itemName + "]", GUIManager.JournalType.Item);
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
		Movement();
		Utilities();
	}
}