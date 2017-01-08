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
	public GameObject lightCircle;

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
	public GameObject rejuvenationPotionPrefab;

	[Header("Weapons")]
	public GameObject woodenSwordPrefab;
	public GameObject ironSwordPrefab;
	public GameObject diamondSwordPrefab;
	public GameObject rubySwordPrefab;
	public GameObject emeraldSwordPrefab;

	[Header("Spells")]
	public GameObject fireballSpellPrefab;
	public GameObject rejuvenationSpellPrefab;

	[Header("Containers")]
	public GameObject chestGoldenPrefab;


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
		listOfPotions.Add(rejuvenationPotionPrefab);

		// spells
		listOfSpells.Add(fireballSpellPrefab);
		listOfSpells.Add(rejuvenationSpellPrefab);
	}

	/// <summary>
	/// Randomizes item.type and returns it.
	/// </summary>
	/// <returns>The loot.</returns>
	public Item.Type RandomizeItemType() {
		System.Array values = System.Enum.GetValues(typeof(Item.Type));
		return (Item.Type) values.GetValue(Random.Range(0, values.Length));
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
			if(item == null || item.GetComponent<Item>() == null) continue;
			if(item.GetComponent<Item>().owner == null) {
				Destroy(item);
			}
		}
		itemInstances.Clear();
	}

	public void MoveActorToPos(Vector2 target, GameObject actor) {

		int zLevel = 0;

		actor.GetComponent<Actor>().position = target;

		if(actor.GetComponent<Player>() != null) zLevel = GameMaster.instance.playerZLevel;
		else if(actor.GetComponent<Enemy>() != null) zLevel = GameMaster.instance.enemyZLevel;

		actor.transform.position = new Vector3(target.x, target.y, zLevel);

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(target);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = actor;

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

	public void InstantiateRandomItemInCategory(Item.Type itemType, Vector2 pos, Item.Rarity rarity, bool isShopItem = false, Transform parent = null) {

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
			break;

		case Item.Type.Gold:
			prefab = listOfGold[Random.Range(0, listOfGold.Count)];
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
			break;
		case Item.Type.Spell:
			prefab = listOfSpells[Random.Range(0, listOfSpells.Count)];
			break;
		case Item.Type.Container:
			prefab = chestGoldenPrefab;
			break;
		}

		if(prefab == null) return;

		// instantiate object.
		instObj = (GameObject) Instantiate(prefab);

		// creates item that takes money before picking up.
		if(isShopItem) {
			instObj.GetComponent<Item>().myState = Item.State.Shop;
		}

		GameObject tileGO = DungeonGenerator.instance.GetTileAtPos(pos);
		Tile tile = tileGO.GetComponent<Tile>();

		// attach item to tile.
		tile.item = instObj;

		// update positions.
		instObj.GetComponent<Item>().position = pos;
		instObj.transform.position = new Vector3(pos.x, pos.y, GameMaster.instance.itemZLevel);

		// set owner to null.
		instObj.GetComponent<Item>().owner = null;

		// save startcolor.
		instObj.GetComponent<Item>().startColor = instObj.GetComponentInChildren<SpriteRenderer>().color;

		// parent
		instObj.transform.SetParent(parent);

		// add to list.
		itemInstances.Add(instObj);

	}

	public void InstantiateEnemyAtPos(int x, int y) {
		Vector2 spawnPos = new Vector2(x, y);

		GameObject enemyInst = (GameObject) Instantiate(enemyPrefab, new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.enemyZLevel), Quaternion.identity);

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = enemyInst;

		enemyInst.GetComponent<Enemy>().position = tile.position;
		enemyInstances.Add(enemyInst);

		GameMaster.instance.enemyCount++;
	}

	public void InstantiateEnemy() {
		Vector2 spawnPos = GetFreeInstPosition();

		GameObject enemyInst = (GameObject) Instantiate(enemyPrefab, new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.enemyZLevel), Quaternion.identity);

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
		//enemyInst.GetComponent<Health>().maxHealth = GameMaster.instance.dungeonLevel;
		//enemyInst.GetComponent<Health>().UpdateCurrentHealth();

		GameMaster.instance.enemyCount++;
	}

	public void InstantiateEnemies() {
		for(int i = 0; i < GameMaster.instance.maxEnemyCountPerDungeon; i++) {
			InstantiateEnemy();
		}
	}

	public void InstantiatePlayer(string pname) {
		Vector2 spawnPos = GetFreeInstPosition();
	
		playerInstance = (GameObject) Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.playerZLevel), Quaternion.identity);
		playerInstance.GetComponent<Player>().position = spawnPos;

		playerInstance.name = pname;
		playerInstance.GetComponent<Actor>().actorName = pname;

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = playerInstance;
	}
}
