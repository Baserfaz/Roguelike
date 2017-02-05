using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuiItemAction : MonoBehaviour {
	
	void Start () {
		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => Action());
	}

	private void Action() {

		// get the item
		GameObject itemGo = GetComponentInParent<InventoryGuiItem>().realItem;

		if(itemGo != null) {

			Inventory inventory = PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>();

			// try to equip/use the item.
			inventory.EquipItem(itemGo);

			// update GUI
			GUIManager.instance.UpdateAllElements();

			// remove item from inventory
			// after it is equiped.
			inventory.removeFromInventory(itemGo);
			GUIManager.instance.RemoveItemFromInventoryListGui(itemGo);

			// destroy the GUI element
			Destroy(this.transform.parent.gameObject);

		} else {
			Debug.LogError("Item is null!"); 
		}
	}
}
