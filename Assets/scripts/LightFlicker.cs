using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour {

	private GameObject lightPrefab;
	private GameObject lightInstance;

	private Color startColor;

	void Awake() {
		lightPrefab = PrefabManager.instance.lightCircle;
		CreateLightInstance();
	}

	private void CreateLightInstance() {
		lightInstance = (GameObject) Instantiate(lightPrefab);
		float randomScale = Random.Range(1f, 1.5f);
		lightInstance.transform.localScale = new Vector3(randomScale, randomScale, lightInstance.transform.localScale.z);
		lightInstance.transform.position = new Vector3(transform.position.x, transform.position.y, GameMaster.instance.lightZLevel);
		lightInstance.transform.SetParent(this.transform);
		startColor = lightInstance.GetComponent<SpriteRenderer>().color;
		Hide();	
	}

	private void Flicker() {
		float randomAlpha = Random.Range(0.1f, startColor.a);
		lightInstance.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, randomAlpha);
	}

	public void Hide() {
		if(lightInstance == null) return;
		lightInstance.GetComponent<SpriteRenderer>().color = Color.clear;
		CancelInvoke();
	}

	public void Show() {
		if(lightInstance == null) return;
		lightInstance.GetComponent<SpriteRenderer>().color = startColor;
		CancelInvoke();
		InvokeRepeating("Flicker", 0f, Random.Range(0.2f, 0.35f));
	}

}
