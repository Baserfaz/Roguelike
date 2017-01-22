using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {

	public SpriteManager.SpriteType mySprite;

	void Awake() {
		GetComponent<SpriteRenderer>().sprite = SpriteManager.instance.CreateTexture(mySprite);
	}
}
