using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryGuiItem : MonoBehaviour, IPointerClickHandler {

	private Color startColor;
	private Color chosenColor = new Color(255, 255, 255, 0.3f);

	public GameObject realItem = null;

	void Start() {
		startColor = GetComponent<Image>().color;
	}

	public void OnPointerClick(PointerEventData data) {

		// our item

		Item item = realItem.GetComponent<Item>();

		// set this to be the chosen GUI object.
		GUIManager.instance.ChangeChosenInventoryGuiItem(this.gameObject);

		// set visual effect
		GetComponent<Image>().color = chosenColor;

		// update info panel's text.
		GUIManager.instance.invItemInfo.GetComponent<Text>().text = 
			"Name: " + item.itemName + 
			"\nDescription: " + item.itemDescription;

		// TODO:
		// 1. what kind of item is it?
		// -> show different kinds of info.


	}

	public void ResetChosenEffect() {
		GetComponent<Image>().color = startColor;
	}
}
