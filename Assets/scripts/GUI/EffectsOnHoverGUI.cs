using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class EffectsOnHoverGUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	[Header("Text settings")]
	public bool hasText = false;
	public string hovertext = "";
	public int fontSize = 32;

	[Header("Sound settings")]
	public bool hasSound = true;
	public SoundManager.Sound hoverSoundType = SoundManager.Sound.GUIHoverOver;
	public SoundManager.Sound clickSoundType = SoundManager.Sound.GUIClick;

	// text instance.
	private GameObject inst = null;

	void Start() {
		DimCanvasGroupAlpha();
	}

	public void OnPointerEnter(PointerEventData data) {
		BrightenCanvasGroupAlpha();

		if(hasSound) {
			SoundManager.instance.PlaySound(hoverSoundType);
		}

		if(hasText) {
			// create or show text obj.
			if(inst == null) {
				inst = (GameObject) Instantiate(GUIManager.instance.hoverTextPrefab);
				inst.transform.SetParent(GUIManager.instance.gameGUI.transform);
				CalculatePosition(inst);
			} else {
				inst.GetComponent<CanvasGroup>().alpha = 1f;
			}

			// set text.
			inst.GetComponent<Text>().text = "<size=" + fontSize + ">" + hovertext + "</size>";

		}
	}

	public void OnPointerExit(PointerEventData data) {
		DimCanvasGroupAlpha();

		if(hasText) {
			inst.GetComponent<CanvasGroup>().alpha = 0f;
		}
	}

	public void OnPointerClick(PointerEventData data) {

		if(hasSound) {
			SoundManager.instance.PlaySound(clickSoundType);
		}

	}

	private void BrightenCanvasGroupAlpha() {
		GetComponent<CanvasGroup>().alpha = 1f;
	}

	private void DimCanvasGroupAlpha() {
		GetComponent<CanvasGroup>().alpha = 0.8f;
	}

	private void CalculatePosition(GameObject hoverTextObj) {
		// calculate the correct position
		// -> if on the right side of the screen, then move text left.
		// -> if on the left side of the screen, then move thext right.
		Vector2 mousePos = Input.mousePosition;
		Vector2 offset = Vector2.zero;

		// modify by using x-axel
		if(mousePos.x > Screen.width/2f) {
			offset = new Vector2(100f, 0f);
		}

		// modify by using y-axel.
		if(mousePos.y > Screen.height/2f) {
			offset += new Vector2(0f, 100f);
		} else {
			offset -= new Vector2(0f, 100f);
		}

		// set the position.
		hoverTextObj.transform.position = mousePos - offset;
	}

}
