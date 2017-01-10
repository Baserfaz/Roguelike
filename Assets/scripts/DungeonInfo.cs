using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInfo : MonoBehaviour {

	private int dungeonHeight = 0;
	private int dungeonWidth = 0;

	public int GetHeight() { return dungeonHeight; }
	public int GetWidth() { return dungeonWidth; }

	public void SetHeight(int a) { dungeonHeight = a; }
	public void SetWidth(int a) { dungeonWidth = a; }

}
