using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

	public static PrefabManager instance;

	private GameObject playerInstance;
	private List<GameObject> enemyInstances = new List<GameObject>();
	private List<GameObject> itemInstances = new List<GameObject>();

	[Header("Actor prefabs")]
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	[Header("Other prefabs")]
	public GameObject blobShadow;

	[Header("Item prefabs")]
	[Header("Armor")]
	public GameObject woodenArmorPrefab;
	public GameObject ironArmorPrefab;
	public GameObject plateArmorPrefab;
	public GameObject rubyArmorPrefab;
	public GameObject emeraldArmorPrefab;

	[Header("Gold")]
	public GameObject goldCoinPrefab;
	public GameObject goldPilePrefab;

	[Header("Potions")]
	public GameObject healthPotionPrefab;
	public GameObject maxHPotionPrefab;
	public GameObject attackPotionPrefab;
	public GameObject armorPotionPrefab;
	public GameObject hurtPotionPrefab;
	public GameObject awesomePotionPrefab;

	[Header("Weapons")]
	public GameObject woodenSwordPrefab;
	public GameObject ironSwordPrefab;
	public GameObject diamondSwordPrefab;
	public GameObject rubySwordPrefab;
	public GameObject emeraldSwordPrefab;

	[Header("Spells")]
	public GameObject fireballSpellPrefab;


	// lists of single items.
	// --> prefabs!
	private List<GameObject> listOfArmors = new List<GameObject>();
	private List<GameObject> listOfGold = new List<GameObject>();
	private List<GameObject> listOfWeapons = new List<GameObject>();
	private List<GameObject> listOfPotions = new List<GameObject>();
	private List<GameObject> listOfSpells = new List<GameObject>();

	void Awake() { instance = this; }
	public GameObject GetPlayerInstance() { return playerInstance; }
	public List<GameObject> GetEnemyInstances() { return enemyInstances; }

	public void ClearItemLists() {
		listOfArmors.Clear();
		listOfGold.Clear();
		listOfWeapons.Clear();
		listOfPotions.Clear();
		listOfSpells.Clear();
	}

	public void PopulateItemLists() {
		// armors
		listOfArmors.Add(woodenArmorPrefab);
		listOfArmors.Add(ironArmorPrefab);
		listOfArmors.Add(plateArmorPrefab);
		listOfArmors.Add(rubyArmorPrefab);
		listOfArmors.Add(emeraldArmorPrefab);

		// gold
		listOfGold.Add(goldCoinPrefab);
		listOfGold.Add(goldPilePrefab);

		// weapons
		listOfWeapons.Add(woodenSwordPrefab);
		listOfWeapons.Add(ironSwordPrefab);
		listOfWeapons.Add(diamondSwordPrefab);
		listOfWeapons.Add(rubySwordPrefab);
		listOfWeapons.Add(emeraldSwordPrefab);

		// potions
		listOfPotions.Add(healthPotionPrefab);
		listOfPotions.Add(maxHPotionPrefab);
		listOfPotions.Add(attackPotionPrefab);
		listOfPotions.Add(armorPotionPrefab);
		listOfPotions.Add(awesomePotionPrefab);
		listOfPotions.Add(hurtPotionPrefab);

		// spells
		listOfSpells.Add(fireballSpellPrefab);
	}

	public void RemovePlayer() {
		Destroy(playerInstance);
	}

	public void RemoveEnemies() {
		foreach(GameObject enemy in enemyInstances) {
			Destroy(enemy);
		}
		enemyInstances.Clear();
	}

	public void RemoveItemFromList(GameObject item){
		itemInstances.Remove(item);
	}

	public void RemoveItems() {
		foreach(GameObject item in itemInstances) {
			if(item == null || item.GetComponent<Item>() == null || item.GetComponent<Item>().owner == null) continue;
			if(item.GetComponent<Item>().owner.GetComponent<Player>() == null) {
				Destroy(item);
			}
		}
		itemInstances.Clear();
	}

	public void MovePlayerToNewStartLocation() {
		Vector2 spawnPos = GetFreeInstPosition();

		playerInstance.GetComponent<Player>().position = spawnPos;
		playerInstance.transform.position = new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.playerZLevel);

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = playerInstance;
	}

	private Vector2 GetFreeInstPosition() {
		List<GameObject> possibleTiles = new List<GameObject>();
		foreach(GameObject tileGo in DungeonGenerator.instance.GetTiles()) {
			Tile tile = tileGo.GetComponent<Tile>();

			if(tile.actor != null) continue;

			if(tile.myType == Tile.TileType.Floor) {
				possibleTiles.Add(tileGo);
			}
		}
		return possibleTiles[Random.Range(0, possibleTiles.Count - 1)].GetComponent<Tile>().position;
	}

	public void InstantiateRandomItemInCategory(Item.Type itemType, Vector2 pos, Item.Rarity rarity) {

		GameObject instObj = null;
		GameObject prefab = null;

		int safetyCounter = 0;
		int safetyCount = 50;

		switch(itemType) {
		case Item.Type.Armor:

			while(true) {
				prefab = listOfArmors[Random.Range(0, listOfArmors.Count)];
				if(prefab.GetComponent<Item>().myRarity == rarity) {
					break;
				}

				if(safetyCounter > listOfArmors.Count + safetyCount) {
					prefab = null;
					break;
				}

				safetyCounter++;

			}

			if(prefab == null) break;

			instObj = (GameObject) Instantiate(prefab);
			break;

		case Item.Type.Gold:
			instObj = (GameObject) Instantiate(listOfGold[Random.Range(0, listOfGold.Count)]);
			break;

		case Item.Type.Weapon:

			while(true) {
				prefab = listOfWeapons[Random.Range(0, listOfWeapons.Count)];
				if(prefab.GetComponent<Item>().myRarity == rarity) {
					break;
				}

				if(safetyCounter > listOfWeapons.Count + safetyCount) {
					prefab = null;
					break;
				}

				safetyCounter++;
			}

			if(prefab == null) break;

			instObj = (GameObject) Instantiate(prefab);
			break;

		case Item.Type.UsableItem:

			// TODO:
			// randomize if it spawns potions or some other use item.
			// now only spawn potions.

			while(true) {
				prefab = listOfPotions[Random.Range(0, listOfPotions.Count)];
				if(prefab.GetComponent<Item>().myRarity == rarity) {
					break;
				}

				if(safetyCounter > listOfPotions.Count + safetyCount) {
					prefab = null;
					break;
				}

				safetyCounter++;
			}

			if(prefab == null) break;

			instObj = (GameObject) Instantiate(prefab);
			break;
		case Item.Type.Spell:
			prefab = listOfSpells[Random.Range(0, listOfSpells.Count)];
			instObj = (GameObject) Instantiate(prefab);
			break;
		}

		if(instObj == null) return;

		GameObject tileGO = DungeonGenerator.instance.GetTileAtPos(pos);
		Tile tile = tileGO.GetComponent<Tile>();

		tile.item = instObj;

		instObj.transform.position = new Vector3(pos.x, pos.y, GameMaster.instance.itemZLevel);
		instObj.GetComponent<Item>().owner = null;
		instObj.GetComponent<Item>().startColor = instObj.GetComponentInChildren<SpriteRenderer>().color;
		itemInstances.Add(instObj);

	}

	public void InstantiateEnemies() {
		for(int i = 0; i < GameMaster.instance.maxEnemyCountPerDungeon; i++) {
			Vector2 spawnPos = GetFreeInstPosition();

			// TODO: 
			// 1. failsafe if spawnpos cant be found.

			GameObject enemyInst = (GameObject) Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

			GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
			Tile tile = tileGo.GetComponent<Tile>();

			tile.actor = enemyInst;

			enemyInst.GetComponent<Enemy>().position = tile.position;
			enemyInstances.Add(enemyInst);

			// TODO:
			// 1. balancing shit

			// 1. damage
			//enemyActor.defaultDamage = GameMaster.instance.dungeonLevel;

			// 2. armor
			//enemyActor.defaultArmor = GameMaster.instance.dungeonLevel;

			// 3. health
			enemyInst.GetComponent<Health>().maxHealth = GameMaster.instance.dungeonLevel;
			enemyInst.GetComponent<Health>().UpdateCurrentHealth();

		}
		// the count of enemies actually (successfully) instantiated.
		GameMaster.instance.enemyCount = enemyInstances.Count;
	}

	public void InstantiatePlayer() {
		Vector2 spawnPos = GetFreeInstPosition();
	
		playerInstance = (GameObject) Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.playerZLevel), Quaternion.identity);
		playerInstance.GetComponent<Player>().position = spawnPos;

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = playerInstance;
	}
}
