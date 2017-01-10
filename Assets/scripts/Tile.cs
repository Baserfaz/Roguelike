using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public Tile pathfindingParentTile = null;

	public enum TileType { Floor, Wall, Empty, Exit, OuterWall, FloorSpecialItem, DoorOpen, DoorClosed, Trap }

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
	public Color GetStartColor() { return startColor; }
	public void ShowAsDiscovered() { GetComponentInChildren<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, 0.5f); }
	public void Show() { GetComponentInChildren<SpriteRenderer>().color = startColor; }
	public void Hide() { GetComponentInChildren<SpriteRenderer>().color = Color.clear; }

}
