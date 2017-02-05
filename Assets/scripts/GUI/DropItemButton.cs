using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropItemButton : MonoBehaviour {

	void Start () {
		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => StartDropItemSequence());
	}

	private void DropItem(Tile tile, Player player) {

		// drop item underneath us.
		tile.item = GetComponentInParent<InventoryGuiItem>().realItem;

		// no one owns the item anymore.
		tile.item.GetComponent<Item>().owner = null;

		// position the item
		tile.item.transform.position = new Vector3(
			tile.position.x, tile.position.y, GameMaster.instance.itemZLevel);

		// set the item's internal position
		tile.item.GetComponent<Item>().position = tile.position;

		// show item. "make it visible"
		tile.item.GetComponent<Item>().ShowItem();

		// remove item from inventory
		player.GetComponent<Inventory>().removeFromInventory(tile.item);

		// remove item from GUI inventory
		GUIManager.instance.RemoveItemFromInventoryListGui(tile.item);

		// destroy the GUI element itself.
		Destroy(this.transform.parent.gameObject);
	}

	private void StartDropItemSequence() {

		Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();
		Tile tile = DungeonGenerator.instance.GetTileAtPos(player.position).GetComponent<Tile>();

		if(tile.item == null) {
			
			// drop item.
			DropItem(tile, player);

		} else {

			// if the spot under us is already taken
			// -> search next free tile.
			GameObject nextTileGo = DungeonGenerator.instance.GetFirstFreeTileNearPosition(player.position);

			if(nextTileGo != null) {

				Tile nextTile = nextTileGo.GetComponent<Tile>();

				// drop item.
				DropItem(nextTile, player);

			} else {
				// can't drop item.
				// no free space anywhere!!
				GUIManager.instance.CreateJournalEntry("Can't drop item, no free space!",
					GUIManager.JournalType.System);

				// sound effect
				SoundManager.instance.PlaySound(SoundManager.Sound.Error);
			}
		}
	}
}
