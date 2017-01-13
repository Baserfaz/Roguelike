using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraEffects : MonoBehaviour {

	private float startVignette = 0f;
	private VignetteAndChromaticAberration v;

	void Awake() { 
		v = GetComponent<VignetteAndChromaticAberration>();
		startVignette = v.intensity; 
	}

	public void DisableAberration() { v.enabled = false; }
	public void EnableAberration() { v.enabled = true; }

	public void StartBrightUp(bool outwards = true) {
		v.intensity = 1f;
		StartCoroutine(BrightUp(outwards));
	}

	private IEnumerator BrightUp(bool outwards) {
		float currentTime = 0f;
		float maxTime = 1f;

		float start = 0f;
		float end = 0f;

		if(outwards) {
			start = 1f;
			end = startVignette;
		} else {
			start = startVignette;
			end = 1f;
		}
		while(currentTime < maxTime) {
			currentTime += Time.deltaTime;
			v.intensity = Mathf.Lerp(start, end, currentTime/maxTime);
			yield return null;
		}
		v.intensity = startVignette;
	}
}
