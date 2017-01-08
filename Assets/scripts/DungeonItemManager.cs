using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonItemManager : MonoBehaviour {

	public static DungeonItemManager instance;

	[Header("Dungeon item settings")]
	public bool allowItemSpawns = true;
	public bool allowContainerSpawns = true;
	public bool allowGoldSpawns = true;
	[Range(0, 100)] public int chanceToSpawnContainer = 10;
	[Range(0, 100)] public int chanceToSpawnGold = 10;

	private GameObject parentGo = null;

	void Awake() { instance = this; }

	public void SpawnItems() {
		if(allowItemSpawns == false) return;

		foreach(GameObject tileGo in DungeonGenerator.instance.GetTiles()) {

			if(parentGo == null) parentGo = (GameObject) new GameObject("Items");

			Tile current = tileGo.GetComponent<Tile>();

			// if tile is not floor 
			// or it's a trap.
			if(current.myType != Tile.TileType.Floor || (current.GetComponent<Trap>() != null)) continue;

			if(current.item == null) {
				
				if(allowContainerSpawns) {
					if(Random.Range(0, 100) > 100 - chanceToSpawnContainer) {

						// delete vanityItem on the same tile.
						if(current.vanityItem != null) {
							Destroy(current.vanityItem);
							DungeonGenerator.instance.UpdateVanityItem(current.position, null);
						}

						PrefabManager.instance.InstantiateRandomItemInCategory(
							Item.Type.Container, current.position, Item.Rarity.Normal, false, parentGo.transform);

						continue;
					} 
				} 

				if(allowGoldSpawns) {
					if(Random.Range(0, 100) > 100 - chanceToSpawnGold) {

						PrefabManager.instance.InstantiateRandomItemInCategory(
							Item.Type.Gold, current.position, Item.Rarity.Normal, false, parentGo.transform);

					}
				}

			}
		}

	}
}
