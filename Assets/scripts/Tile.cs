using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum TileType { Floor, Wall, Empty, Exit, OuterWall, FloorSpecialItem, DoorOpen, DoorClosed }

	public Vector2 position;
	public TileType myType;
	public SpriteManager.SpriteType mySpriteType;

	public GameObject actor = null;
	public GameObject item = null;
	public GameObject vanityItem = null;

	public bool isDiscovered = false;
	public bool isVisible = false;

	private Color startColor;

	void Awake() { startColor = GetComponentInChildren<SpriteRenderer>().color; }

	public void SetStartColor(Color newColor) { startColor = newColor; }
	public void ShowAsDiscovered() { GetComponentInChildren<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, 0.5f); }
	public void Show() { GetComponentInChildren<SpriteRenderer>().color = startColor; }
	public void Hide() { GetComponentInChildren<SpriteRenderer>().color = Color.clear; }

	// TODO
	// Refactor door methods.
	// Door : Tile perhaps??

	public void OpenDoor() {
		
		myType = TileType.DoorOpen;

		if(mySpriteType == SpriteManager.SpriteType.DoorHorizontalClosed) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorHorizontalOpen);
		} else if(mySpriteType == SpriteManager.SpriteType.DoorVerticalClosed) {
			GetComponentInChildren<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.DoorVerticalOpen);
		}

	}
}
