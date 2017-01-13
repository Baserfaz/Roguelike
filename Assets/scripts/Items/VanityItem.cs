using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanityItem : MonoBehaviour {

	public Vector2 position;

	public void HideItem() { 
		GetComponentInChildren<SpriteRenderer>().color = Color.clear; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Hide();
		if(GetComponent<LightFlicker>() != null) GetComponent<LightFlicker>().Hide();
	}
	public void ShowItem() {
		GetComponentInChildren<SpriteRenderer>().color = Color.white; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Show();
		if(GetComponent<LightFlicker>() != null) GetComponent<LightFlicker>().Show();
	}

}
