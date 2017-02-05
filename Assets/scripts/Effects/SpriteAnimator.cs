using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpriteAnimator : MonoBehaviour {

	[Header("Frames")]
	public Sprite[] frames;

	[Header("Settings")]
	public float timeToLive = 1f;
	public bool canLoop = true;
	public int animationSpeed = 3;

	private float timer = 0f;
	private int pointer = 0;

	void Start() { 
		timer = Time.time + timeToLive;
	}

	void Update() {
		
		// change sprite
		if(Time.frameCount % animationSpeed == 0) {
			ChangeSprite();
		}

		// destroy after some time.
		if(timer < Time.time) {
			Destroy(this.gameObject);
		}
	}

	private void ChangeSprite() {
		try {
			GetComponent<SpriteRenderer>().sprite = frames[pointer];
			pointer++;
		} catch(IndexOutOfRangeException) {
			if(canLoop) pointer = 0;
			else Destroy(this.gameObject);
		}
	}
}
