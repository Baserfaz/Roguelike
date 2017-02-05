using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour {

	void Start () {
		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => ToggleInventory());
	}

	private void ToggleInventory() {
		// this button only opens the inventory.
		if(GUIManager.instance.getIsInventoryOpen() == false) OpenInventory();
	}

	private void OpenInventory() {

		// open gui.
		GUIManager.instance.setIsInventoryOpen(true);
		GUIManager.instance.ShowInventory();

		// change the mouse state.
		MouseController.instance.ChangeState(MouseController.State.OnGui);
	
		// populate inventory list in GUI view.
		GUIManager.instance.CreateInventoryListItems();
	}
}
