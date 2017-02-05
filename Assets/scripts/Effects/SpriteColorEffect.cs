using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorEffect : MonoBehaviour {

	private IEnumerator PulseRed() {

		float currentTime = 0f;
		float maxTime = 0.3f;

		// get the reference.
		SpriteRenderer sr = transform.GetChild (0).GetComponent<SpriteRenderer> ();

		Color endColor = sr.color;
		Color startColor = Color.red;

		// set the color to red and
		// lerp it back to normal color.
		sr.color = startColor;

		while (currentTime < maxTime) {

			currentTime += Time.deltaTime;

			sr.color = Color.Lerp (startColor, endColor, currentTime/maxTime);

			yield return null;
		}

		sr.color = endColor;
	}

	public void StartPulseRed() {
		StopAllCoroutines ();
		StartCoroutine (PulseRed ());
	}
}
