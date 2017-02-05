using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	private AudioSource audiosource;

	public enum Sound { Attack, PickUpItem, BuyItem,
		MoveActor, PickUpCoin, CastSpell, useItem, Error, Hurt, OpenChest,
		Miss, Heal, Pass, LvlUp, OpenDoor, GUIHoverOver, GUIClick, None }

	[Header("Sound Settings")]
	public float maxVolume = 1f;
	public float minVolume = 0.1f;
	public float muteVolume = 0f;

	[Header("Sound files")]
	public AudioClip sAttack;
	public AudioClip sPickUpItem;
	public AudioClip sBuyItem;
	public AudioClip sMoveActor;
	public AudioClip sPickupCoin;
	public AudioClip sCastSpell;
	public AudioClip sUseItem;
	public AudioClip sError;
	public AudioClip sHurt;
	public AudioClip sOpenChest;
	public AudioClip sMiss;
	public AudioClip sHeal;
	public AudioClip sPass;
	public AudioClip sLvlup;
	public AudioClip sOpenDoor;
	public AudioClip sGuiHover;
	public AudioClip sGuiClick;


	void Awake() {
		instance = this;
		audiosource = GetComponent<AudioSource>();
	}

	public void ChangeVolume(float vol) {
		GetComponent<AudioSource>().volume = vol;
	}

	public void PlaySound(Sound sound) {

		AudioClip clip = new AudioClip();

		switch(sound) {

		case Sound.OpenDoor:
			clip = sOpenDoor;
			break;

		case Sound.LvlUp:
			clip = sLvlup;
			break;

		case Sound.Miss:
			clip = sMiss;
			break;

		case Sound.Heal:
			clip = sHeal;
			break;

		case Sound.Pass:
			clip = sPass;
			break;

		case Sound.OpenChest:
			clip = sOpenChest;
			break;

		case Sound.Attack:
			clip = sAttack;
			break;
		case Sound.BuyItem:
			clip = sBuyItem;
			break;

		case Sound.CastSpell:
			clip = sCastSpell;
			break;

		case Sound.MoveActor:
			clip = sMoveActor;
			break;

		case Sound.PickUpCoin:
			clip = sPickupCoin;
			break;

		case Sound.PickUpItem:
			clip = sPickUpItem;
			break;

		case Sound.useItem:
			clip = sUseItem;
			break;
		
		case Sound.Error:
			clip = sError;
			break;

		case Sound.Hurt:
			clip = sHurt;
			break;

		case Sound.GUIClick:
			clip = sGuiClick;
			break;

		case Sound.GUIHoverOver:
			clip = sGuiHover;
			break;

		case Sound.None:
			break;

		default:
			Debug.LogError("No such sound!");
			break;
		}

		if(clip != null) {
			audiosource.pitch = Random.Range(0.95f, 1.05f);
			audiosource.PlayOneShot(clip);
		}
	}
}
