﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonVanityManager : MonoBehaviour {

	public static DungeonVanityManager instance;

	public enum VanityItem { Candle, Lantern, Skull, Blood }

	[Header("Settings")]
	public bool spawnVanityItems = true;
	public bool randomizeItemOffset = true;
	public float offsetAmount = 0.2f;
	[Range(0, 100)] public int vanityitemCoverage = 10;

	[Header("Vanity item prefabs")]
	public GameObject skull01Prefab;
	public GameObject lantern01Prefab;
	public GameObject burntCandlePrefab;
	public GameObject bloodPrefab;

	private GameObject parentGo;
	private List<GameObject> InstantiatedVanityItems = new List<GameObject>();

	void Awake() { 
		instance = this; 
	}

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
		case VanityItem.Blood:
			vanityItemPrefab = bloodPrefab;
			break;

		default:
			Debug.LogError("No such item.");
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

		InstantiatedVanityItems.Add(inst);
	}

	public void RemoveVanityItems() {
		foreach(GameObject item in InstantiatedVanityItems) {
			Destroy(item);
		}
		InstantiatedVanityItems.Clear();
	}

	private VanityItem RandomVanityItem() {
		System.Array values = System.Enum.GetValues(typeof(VanityItem));
		VanityItem item = (VanityItem) values.GetValue(Random.Range(0, values.Length));
		return item;
	}

	public void SpawnVanityItems() {
		if(spawnVanityItems) {

			foreach(GameObject tileGo in DungeonGenerator.instance.GetTiles()) {
				Tile tile = tileGo.GetComponent<Tile>();

				// if the tile is not floor or it is a trap
				// -> continue.
				if(tile.myType != Tile.TileType.Floor || tileGo.GetComponent<Trap>() != null) continue;

				if(Random.Range(0, 100) > 100 - vanityitemCoverage) {

					if(tile.vanityItem == null) {

						VanityItem item = RandomVanityItem();

						while(item == VanityItem.Blood) {
							item = RandomVanityItem();
						}

						SpawnVanityItem(item, tile.position);

					}
				}
			}
		}
	}


}
