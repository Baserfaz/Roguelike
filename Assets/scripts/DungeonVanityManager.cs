using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonVanityManager : MonoBehaviour {

	public static DungeonVanityManager instance;

	public enum VanityItem { Candle, Lantern, Skull }

	[Header("Settings")]
	public bool spawnVanityItems = true;
	public bool randomizeItemOffset = true;
	public float offsetAmount = 0.2f;
	[Range(0, 100)] public int vanityitemCoverage = 10;

	[Header("Vanity item prefabs")]
	public GameObject skull01Prefab;
	public GameObject lantern01Prefab;
	public GameObject burntCandlePrefab;

	private GameObject parentGo;
	private List<GameObject> vanityItems = new List<GameObject>();

	void Awake() { instance = this; }

	public void SpawnVanityItem(VanityItem item, Vector2 pos) {

		if(parentGo == null) parentGo = new GameObject("VanityItems");

		GameObject vanityItemPrefab = null;

		switch(item) {
		case VanityItem.Candle:
			vanityItemPrefab = burntCandlePrefab;
			break;
		case VanityItem.Lantern:
			vanityItemPrefab = lantern01Prefab;
			break;
		case VanityItem.Skull:
			vanityItemPrefab = skull01Prefab;
			break;
		}
	
		GameObject inst = (GameObject) Instantiate(vanityItemPrefab);
		inst.transform.position = new Vector3(pos.x, pos.y, GameMaster.instance.vanityitemsZLevel);

		if(randomizeItemOffset) {
			Vector2 offset = new Vector2(Random.Range(-offsetAmount, offsetAmount), Random.Range(-offsetAmount, offsetAmount));
			inst.transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, GameMaster.instance.vanityitemsZLevel);
		} else {
			inst.transform.position = new Vector3(pos.x, pos.y, GameMaster.instance.vanityitemsZLevel);
		}

		inst.transform.SetParent(parentGo.transform);

		DungeonGenerator.instance.GetTileAtPos(pos).GetComponent<Tile>().vanityItem = inst;

		vanityItems.Add(inst);

	}

	public void RemoveVanityItems() {
		foreach(GameObject item in vanityItems) {
			Destroy(item);
		}
		vanityItems.Clear();
	}

	public void SpawnVanityItems() {
		if(spawnVanityItems) {

			foreach(GameObject tileGo in DungeonGenerator.instance.GetTiles()) {
				Tile tile = tileGo.GetComponent<Tile>();

				if(tile.myType != Tile.TileType.Floor) continue;

				if(Random.Range(0, 100) > 100 - vanityitemCoverage) {

					if(tile.vanityItem == null) {

						// TODO:
						// randomize item.
						SpawnVanityItem(VanityItem.Candle, tile.position);

					}
				}
			}
		}
	}


}
