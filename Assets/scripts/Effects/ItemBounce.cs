using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBounce : MonoBehaviour {

	private GameObject bounceObject;
	private bool up = true;
	private float currentTime = 0f;
	private float timeBop = 0.75f;
	private float bopSpeed = 0.0015f;

	void Awake() { bounceObject = this.gameObject; }

	void Start() { currentTime = Random.Range(0f, timeBop); }

	void Update() {
		Bounce();
	}

	private void Bounce() {
		currentTime += Time.deltaTime * 0.7f;
		Transform current = bounceObject.transform;

		if(up) {
			current.position += Vector3.up * bopSpeed;

			if(currentTime > timeBop) {
				up = false;
				currentTime = 0f;
			}
		} else {

			current.position -= Vector3.up * bopSpeed;

			if(currentTime > timeBop) {
				up = true;
				currentTime = 0f;
			}
		}
	}
}
