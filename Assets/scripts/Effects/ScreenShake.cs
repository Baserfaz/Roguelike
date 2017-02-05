using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour {

	private float shakeAmount = 0.15f;

	private IEnumerator Shake() {

		float currentTime = 0f;
		float maxTime = 0.5f;

		// references
		Vector3 pos = transform.position;
		Vector3 startpos = pos + new Vector3 (Random.Range(-shakeAmount, shakeAmount),
			Random.Range(-shakeAmount, shakeAmount) , 0f);
		Vector3 endpos = transform.position;

		// offset the camera from center.
		transform.position = startpos;

		// then lerp it back to center.
		while(currentTime < maxTime) {
			currentTime += Time.deltaTime;
			transform.position = Vector3.Lerp (startpos, endpos, currentTime/maxTime);
			yield return null;
		}

		// force set the position after lerp.
		transform.position = endpos;
	}

	public void StartShake() { 
		StopAllCoroutines ();
		StartCoroutine (Shake ());
	
		// we dont actually need to call coroutine,
		// because the camera automatically centers itself.
		// so just offset the position to get shake effect!

		/*Vector3 startpos = transform.position + new Vector3 (Random.Range(-shakeAmount, shakeAmount),
			Random.Range(-shakeAmount, shakeAmount) , 0f);

		transform.position = startpos;*/
	}
}
