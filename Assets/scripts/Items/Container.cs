using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item {

	/// <summary>
	/// Open this instance.
	/// Chest cannot drop legendary or rare loot!
	/// Hardcoded
	/// </summary>
	public void Open() {

		// Detach this item from tile.
		DungeonGenerator.instance.UpdateTileItem(position, null);

		// attach this to vanity item slot.
		DungeonGenerator.instance.UpdateVanityItem(position, this.gameObject);

		VanityItem vi = gameObject.AddComponent<VanityItem>();
		vi.position = position;

		// stop bouncing effect.
		Destroy(GetComponent<ItemBounce>());

		// randomize loot type
		Item.Type randomType = PrefabManager.instance.RandomizeItemType();
		while(randomType == Type.Container) {
			randomType = PrefabManager.instance.RandomizeItemType();
		}

		// randomize loot rarity.
		Item.Rarity randomRarity = PrefabManager.instance.RandomizeItemRarity();
		while(randomRarity == Item.Rarity.Legendary || randomRarity == Item.Rarity.Rare) {
			randomRarity = PrefabManager.instance.RandomizeItemRarity();
		}

		// instantiate random type of loot.
		PrefabManager.instance.InstantiateRandomItemInCategory(randomType, position, randomRarity);

		// Update the sprite.
		updateGraphics();
	}

	private void updateGraphics() {
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
		sr.sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.ChestOpen);
	}

}
