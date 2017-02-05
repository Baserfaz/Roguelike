using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowController : MonoBehaviour {

	private GameObject shadowGo;
	private Color startColor;
	private Vector3 offset = new Vector3(0f, -0.25f, 0f);

	public void Hide() { 
		if (shadowGo != null) {
			shadowGo.GetComponent<SpriteRenderer> ().color = Color.clear; 
		} else {
			InstantiateShadow ();
			Hide ();
		}
	}

	public void Show() {
		if (shadowGo != null) {
			shadowGo.GetComponent<SpriteRenderer> ().color = startColor; 
		} else {
			InstantiateShadow ();
			Show ();
		}
	}

	public void Move() {
		if (shadowGo != null) {
			shadowGo.transform.position = new Vector3 (
				transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z + 0.1f);
		}
	}

	private void InstantiateShadow() {
		startColor = new Color (0f, 0f, 0f, 0.25f);

		shadowGo = new GameObject ("Shadow");
		shadowGo.transform.SetParent (this.transform);
		SpriteRenderer sr = shadowGo.AddComponent<SpriteRenderer> ();
		sr.sprite = transform.GetChild (0).GetComponent<SpriteRenderer> ().sprite;
		sr.material = MaterialManager.instance.mySpriteMaterial;
		sr.color = startColor;
		sr.flipY = true;
		shadowGo.transform.localScale = transform.GetChild (0).transform.localScale;
		shadowGo.transform.localPosition = new Vector2 (0f, -0.5f);

		if(GetComponent<Gold>() != null || GetComponent<Weapon>() != null) {
			sr.flipX = true;
		}

	}
}
