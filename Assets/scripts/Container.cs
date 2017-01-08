using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item {

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
		// 1. do not instantiate container.
		Item.Type randomType = PrefabManager.instance.RandomizeItemType();
		while(randomType == Type.Container) {
			randomType = PrefabManager.instance.RandomizeItemType();
		}

		// instantiate random type of loot.
		// TODO: randomize rarity.
		PrefabManager.instance.InstantiateRandomItemInCategory(randomType, position, Rarity.Normal);

		// Update the sprite.
		updateGraphics();
	}

	private void updateGraphics() {
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
		sr.sprite = SpriteManager.instance.CreateTexture(SpriteManager.SpriteType.ChestOpen);
	}

}
