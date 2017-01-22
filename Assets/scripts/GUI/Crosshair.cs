using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	[HideInInspector] public Vector2 position;

	public void MoveCrosshair(Vector2 pos) {
		position = pos;
		transform.position = position;
	}

	public void HideCursor() { GetComponent<SpriteRenderer>().color = Color.clear; }
	public void ShowCursor() { GetComponent<SpriteRenderer>().color = Color.white; }

	public void MoveCursor(Tile tile) {
		ShowCursor();
		transform.position = tile.position;
		HandleTileInformation(tile);
	}

	/// <summary>
	/// Handles the tile information.
	/// </summary>
	/// <param name="tile">Tile to be handled.</param>
	private void HandleTileInformation(Tile tile) {

		// update our little info gui element.
		GUIManager.instance.ShowTileInfo(tile);

		// we are hovering on a tile which has the player.
		// create a popup text to tell the player that a turn can
		// be passed by clicking.

		// TODO:

	}
}
