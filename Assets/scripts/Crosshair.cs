using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	[HideInInspector] public Vector2 position;

	public void MoveCrosshair(Vector2 pos) {
		position = pos;
		transform.position = position;
	}

}
