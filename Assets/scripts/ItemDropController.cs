using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropController : MonoBehaviour {

	public static ItemDropController instance;

	void Awake() { instance = this; }

	[Header("Item drop chances from enemies.")]
	[Range(0, 100)] public int itemDropChance = 100;
	[Range(0, 100)] public int armorDropChance = 5;
	[Range(0, 100)] public int weaponDropChance = 5;
	[Range(0, 100)] public int useItemDropChance = 10;
	[Range(0, 100)] public int spellDropChance = 10;

	[Header("Rarity chances")]
	[Range(0, 100)] public int normalChance = 50;
	[Range(0, 100)] public int magicalChance = 20;
	[Range(0, 100)] public int rareChance = 10;
	[Range(0, 100)] public int legendaryChance = 1;

	public void DropItem(Vector2 pos) {

		// drop item at all.
		if(Random.Range(0, 100) > 100 - itemDropChance) {

			// drop armor.
			if(Random.Range(0, 100) > 100 - armorDropChance) {

				// rarities.
				if(Random.Range(0, 100) > 100 - normalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Armor, pos, Item.Rarity.Normal);

				} else if(Random.Range(0, 100) > 100 - magicalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Armor, pos, Item.Rarity.Magical);

				} else if(Random.Range(0, 100) > 100 - rareChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Armor, pos, Item.Rarity.Rare);

				} else if(Random.Range(0, 100) > 100 - legendaryChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Armor, pos, Item.Rarity.Legendary);

				}

				// drop weapon
			} else if(Random.Range(0, 100) > 100 - weaponDropChance) {

				// rarities.
				if(Random.Range(0, 100) > 100 - normalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Weapon, pos, Item.Rarity.Normal);

				} else if(Random.Range(0, 100) > 100 - magicalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Weapon, pos, Item.Rarity.Magical);

				} else if(Random.Range(0, 100) > 100 - rareChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Weapon, pos, Item.Rarity.Rare);

				} else if(Random.Range(0, 100) > 100 - legendaryChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Weapon, pos, Item.Rarity.Legendary);

				}

				// drop use item
			} else if(Random.Range(0, 100) > 100 - useItemDropChance) {

				// TODO:
				// randomize if it's potion or some other item type, such as magical item.

				// rarities.
				if(Random.Range(0, 100) > 100 - normalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.UsableItem, pos, Item.Rarity.Normal);

				} else if(Random.Range(0, 100) > 100 - magicalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.UsableItem, pos, Item.Rarity.Magical);

				} else if(Random.Range(0, 100) > 100 - rareChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.UsableItem, pos, Item.Rarity.Rare);

				} else if(Random.Range(0, 100) > 100 - legendaryChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.UsableItem, pos, Item.Rarity.Legendary);

				}

				//spell drop
			} else if(Random.Range(0, 100) > 100 - spellDropChance) {
				
				// rarities.
				if(Random.Range(0, 100) > 100 - normalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Spell, pos, Item.Rarity.Normal);

				} else if(Random.Range(0, 100) > 100 - magicalChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Spell, pos, Item.Rarity.Magical);

				} else if(Random.Range(0, 100) > 100 - rareChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Spell, pos, Item.Rarity.Rare);

				} else if(Random.Range(0, 100) > 100 - legendaryChance) {

					PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Spell, pos, Item.Rarity.Legendary);

				}

			} else {

				PrefabManager.instance.InstantiateRandomItemInCategory(Item.Type.Gold, pos, Item.Rarity.Normal);

			}

		} 
	}


}
