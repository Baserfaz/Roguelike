using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour {

	public static CrosshairManager instance;

	[Header("Crosshair settings")]
	public GameObject crosshairPrefab;

	private GameObject crosshairInstance = null;

	void Awake() { instance = this; }

	public void CrosshairMode(Vector2 position) {
		InstantiateCrosshair(position);
		GameMaster.instance.movementMode = GameMaster.MovementMode.Crosshair;
	}

	public void PlayerMode() {
		DestroyCrosshair();
		GameMaster.instance.movementMode = GameMaster.MovementMode.Player;
	}

	public GameObject GetCrosshairInstance() {
		return crosshairInstance;
	}

	public void InstantiateCrosshair(Vector2 pos) {
		crosshairInstance = (GameObject) Instantiate(crosshairPrefab);

		Vector3 newPos = new Vector3(pos.x, pos.y, GameMaster.instance.crosshairZLevel);

		crosshairInstance.GetComponent<Crosshair>().position = newPos;
		crosshairInstance.transform.position = newPos;
	}

	public void DestroyCrosshair() {
		if(crosshairInstance != null) Destroy(crosshairInstance);
	}



}
