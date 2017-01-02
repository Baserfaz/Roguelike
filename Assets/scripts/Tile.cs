using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public enum TileType { Floor, Wall, Empty, Exit, OuterWall }

	public Vector2 position;
	public TileType myType;

	public GameObject actor = null;
	public GameObject item = null;

	public bool isDiscovered = false;
	public bool isVisible = false;

	private Color startColor;

	void Awake() { startColor = GetComponentInChildren<SpriteRenderer>().color; }

	public void SetStartColor(Color newColor) { startColor = newColor; }
	public void ShowAsDiscovered() { GetComponentInChildren<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, 0.5f); }
	public void Show() { GetComponentInChildren<SpriteRenderer>().color = startColor; }
	public void Hide() { GetComponentInChildren<SpriteRenderer>().color = Color.clear; }
}
