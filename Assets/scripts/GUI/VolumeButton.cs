using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeButton : MonoBehaviour {

	public enum VolumeLevel { Max, Min, Mute }
	private VolumeLevel volume = VolumeLevel.Max;

	[Header("Sprites")]
	public Sprite sMaxVol;
	public Sprite sMinVol;
	public Sprite sMute;

	void Start () {

		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => ChangeVolume());

		// default volume to max settings
		volume = VolumeLevel.Max;

		// change it.
		ChangeVolume();
	}
	
	private void ChangeVolume() {

		// button's image component.
		Image image = GetComponent<Image>();

		switch(volume) {
		case VolumeLevel.Max:

			volume = VolumeLevel.Min;
			image.sprite = sMaxVol;
			GetComponent<Button>().image = image;
			SoundManager.instance.ChangeVolume(SoundManager.instance.maxVolume);
			break;

		case VolumeLevel.Min:

			volume = VolumeLevel.Mute;
			image.sprite = sMinVol;
			SoundManager.instance.ChangeVolume(SoundManager.instance.minVolume);
			break;

		case VolumeLevel.Mute:

			volume = VolumeLevel.Max;
			image.sprite = sMute;
			SoundManager.instance.ChangeVolume(SoundManager.instance.muteVolume);
			break;
		}

		GetComponent<Button>().image = image;
	}
}
