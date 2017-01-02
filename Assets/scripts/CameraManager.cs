using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public static CameraManager instance;

	private Vector3 targetPosition;
	private int cameraZLevel = -10;

	private int maxZoom = 15;
	private int minZoom = 3;

	void Awake() { instance = this; }

	void Update () {
		if(GameMaster.instance.movementMode == GameMaster.MovementMode.Player) {
			if(PrefabManager.instance.GetPlayerInstance() != null) {
				FollowPlayer();
			}
		} else if(GameMaster.instance.movementMode == GameMaster.MovementMode.Crosshair) {
			if(CrosshairManager.instance.GetCrosshairInstance() != null) {
				FollowCrosshair();
			}
		}
	}

	private void FollowCrosshair() {
		float d = Vector3.Distance(transform.position, targetPosition);
		Vector2 pos = CrosshairManager.instance.GetCrosshairInstance().GetComponent<Crosshair>().position;
		targetPosition = new Vector3(pos.x, pos.y, cameraZLevel);
		if(d > 0.1f) transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
	}

	private void FollowPlayer() {
		float d = Vector3.Distance(transform.position, targetPosition);
		UpdatePosition();
		if(d > 0.1f) transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10f);
	}

	private void UpdatePosition() {
		Vector2 pos = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>().position;
		targetPosition = new Vector3(pos.x, pos.y, cameraZLevel);
	}

	public void ZoomIn() {
		GetComponent<Camera>().orthographicSize -= 0.5f;
		if(GetComponent<Camera>().orthographicSize < minZoom) GetComponent<Camera>().orthographicSize = minZoom;
	}

	public void ZoomOut() {
		GetComponent<Camera>().orthographicSize += 0.5f;
		if(GetComponent<Camera>().orthographicSize > maxZoom) GetComponent<Camera>().orthographicSize = maxZoom;
	}



}



