using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour {

	/// <summary>
	/// Fades text plus moves it upwards.
	/// </summary>
	/// <param name="fadeTime">Fade time.</param>
	public void StartFadeUp(float fadeTime) {
		StartCoroutine(FadeTextUp(fadeTime));
	}

	/// <summary>
	/// Only fades text in place.
	/// </summary>
	/// <param name="fadeTime">Fade time.</param>
	public void StartFade(float fadeTime) {
		StartCoroutine(FadeText(fadeTime));
	}

	private IEnumerator FadeText(float fadeTime) {

		float currentTime = 0f;

		while(currentTime < fadeTime) {

			currentTime += Time.deltaTime;

			float percentageDone = currentTime / fadeTime;

			GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, percentageDone);

			yield return null;
		}

		Destroy(this.gameObject);
	}

	private IEnumerator FadeTextUp(float fadeTime) {

		float currentTime = 0f;
		//float fadeTime = 3f;

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
