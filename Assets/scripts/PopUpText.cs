using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour {

	void Start () {
		StartCoroutine("FadeText");
	}

	private IEnumerator FadeText() {

		float currentTime = 0f;
		float fadeTime = 1f;

		float positionOffset = 0.25f;

		Vector3 startPos = transform.position;
		Vector3 endPos = new Vector3(startPos.x, startPos.y + positionOffset, startPos.z);

		while(currentTime < fadeTime) {

			currentTime += Time.deltaTime;

			float percentageDone = currentTime / fadeTime;

			GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, percentageDone);
			transform.position = Vector3.Lerp(startPos, endPos, percentageDone);

			yield return null;
		}

		Destroy(this.gameObject);
	}

}
