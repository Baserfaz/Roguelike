using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	[Header("Inventory settings")]
	public int maxInvSpace = 20;

	[HideInInspector] public GameObject currentArmor;
	[HideInInspector] public GameObject currentWeapon;
	[HideInInspector] public GameObject currentUseItem;
	[HideInInspector] public GameObject currentSpell;

	[HideInInspector] public List<GameObject> inventory = new List<GameObject>();
	[HideInInspector] public int currentGold = 0;

	public void removeFromInventory(GameObject item) {
		inventory.Remove(item);
	}

	private void AddToInventory(GameObject item) {

		// add item to inventory if it's not full.
		if(inventory.Count < maxInvSpace) {
			inventory.Add(item);
			DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
			item.GetComponent<Item>().HideItem();

			// update gui
			GUIManager.instance.CreateJournalEntry("Picked up " + item.GetComponent<Item>().itemName + ".",
				GUIManager.JournalType.Item);

			// sound effect.
			SoundManager.instance.PlaySound(SoundManager.Sound.PickUpItem);

		} else {

			// gui
			GUIManager.instance.CreateJournalEntry("Inventory is full.", GUIManager.JournalType.System);

			// sound effect.
			SoundManager.instance.PlaySound(SoundManager.Sound.Error);
		}
	}

	public void HandleItem(GameObject itemGo) {

		if(itemGo.GetComponent<Gold>() != null) {

			int amount = itemGo.GetComponent<Gold>().amount;

			currentGold += amount;

			GUIManager.instance.CreatePopUpEntry("+" + amount + "g", GetComponent<Actor>().position, GUIManager.PopUpType.Gold);
			GUIManager.instance.CreateJournalEntry("Picked up " + amount + " gold.", GUIManager.JournalType.Item);

			DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
			Destroy(itemGo.gameObject);

			//sound effects
			SoundManager.instance.PlaySound(SoundManager.Sound.PickUpCoin);
		} else {
			// put the item into inventory.
			itemGo.GetComponent<Item>().owner = this.gameObject;
			AddToInventory(itemGo);
		}
	}

	// equip or use item that is passed as an argument.
	public void EquipItem(GameObject item) {

		if(item.GetComponent<Armor>() != null) {

			if(currentArmor != null) {
				
				// swap the two items.
				currentArmor = item;
				currentArmor.GetComponent<Item>().HideItem();
				
			} else {
				// just pick up the item.
				DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
				currentArmor = item;
				currentArmor.GetComponent<Item>().HideItem();

				//SoundManager.instance.PlaySound(SoundManager.Sound.PickUpItem);
			}

		} else if(item.GetComponent<Weapon>() != null) {

			if(currentWeapon != null) {
				// swap the two items.
				currentWeapon = item;
				currentWeapon.GetComponent<Item>().HideItem();
			} else {
				// just pick up the item.
				DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
				currentWeapon = item;
				currentWeapon.GetComponent<Item>().HideItem();

				//SoundManager.instance.PlaySound(SoundManager.Sound.PickUpItem);
			}

		} else if(item.GetComponent<UseItem>() != null) {

			if(currentUseItem != null) {
				// swap the two items.
				currentUseItem = item;
				currentUseItem.GetComponent<Item>().HideItem();
			} else {
				// just pick up the item.
				DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
				currentUseItem = item;
				currentUseItem.GetComponent<Item>().HideItem();

				//SoundManager.instance.PlaySound(SoundManager.Sound.PickUpItem);
			}

		} else if(item.GetComponent<Spell>() != null) {

			if(currentSpell != null) {
				// swap the two items.
				currentSpell = item;
				currentSpell.GetComponent<Item>().HideItem();
			} else {
				// just pick up the item.
				DungeonGenerator.instance.UpdateTileItem(GetComponent<Actor>().position, null);
				currentSpell = item;
				currentSpell.GetComponent<Item>().HideItem();

				//SoundManager.instance.PlaySound(SoundManager.Sound.PickUpItem);
			}
		} 
	}

	public void DropCurrentItem(Item.Type type) {

		Vector2 currentPosition = GetComponent<Actor>().position;
		Vector3 itemDropPosition = new Vector3(currentPosition.x, currentPosition.y, GameMaster.instance.itemZLevel);

		switch(type) {
		case Item.Type.Armor:
			currentArmor.GetComponent<Item>().ShowItem();
			currentArmor.transform.position = itemDropPosition;
			DungeonGenerator.instance.UpdateTileItem(currentPosition, currentArmor);
			currentArmor.GetComponent<Item>().owner = null;
			break;
		case Item.Type.Weapon:
			currentWeapon.GetComponent<Item>().ShowItem();
			currentWeapon.transform.position = itemDropPosition;
			DungeonGenerator.instance.UpdateTileItem(currentPosition, currentWeapon);
			currentWeapon.GetComponent<Item>().owner = null;
			break;
		case Item.Type.UsableItem:
			currentUseItem.GetComponent<Item>().ShowItem();
			currentUseItem.transform.position = itemDropPosition;
			DungeonGenerator.instance.UpdateTileItem(currentPosition, currentUseItem);
			currentUseItem.GetComponent<Item>().owner = null;
			break;
		case Item.Type.Spell:
			// reset spell's cooldown on drop,
			// so player can't scum multiple spells.
			currentSpell.GetComponent<Spell>().ResetCooldown();
			currentSpell.GetComponent<Item>().ShowItem();
			currentSpell.transform.position = itemDropPosition;
			DungeonGenerator.instance.UpdateTileItem(currentPosition, currentSpell);
			currentSpell.GetComponent<Item>().owner = null;
			break;
		}
	}
}
