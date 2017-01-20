using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shadowController : MonoBehaviour {

	private GameObject shadowGo;
	private Color startColor;
	private Vector3 offset = new Vector3(0f, -0.25f, 0f);
	private Vector3 scale = new Vector3(0.75f, 0.75f, 1f);

	public void Hide() { shadowGo.GetComponent<SpriteRenderer>().color = Color.clear; }
	public void Show() { shadowGo.GetComponent<SpriteRenderer>().color = startColor; }
	public void Move() { shadowGo.transform.position = new Vector3(transform.position.x + offset.x, transform.position.y + offset.y, transform.position.z + 0.1f); }

	void Awake() {
		shadowGo = (GameObject) Instantiate(PrefabManager.instance.blobShadow);
		startColor = shadowGo.GetComponent<SpriteRenderer>().color;
		shadowGo.transform.localScale = scale;
		Move();
		shadowGo.transform.SetParent(this.transform);
		shadowGo.name = "Shadow";
	}
}
