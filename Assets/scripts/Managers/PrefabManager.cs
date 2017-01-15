using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour {

	// this instance.
	public static PrefabManager instance;

	// Instantiated prefabs are stored here.
	private GameObject playerInstance;
	private List<GameObject> enemyInstances = new List<GameObject>();
	private List<GameObject> itemInstances = new List<GameObject>();

	// enemy parent go.
	private GameObject enemyParent = null;

	[Header("Player prefabs")]
	public GameObject playerPrefab;

	[Header("Enemy prefabs")]
	public GameObject impPrefab;
	public GameObject slimeBluePrefab;
	public GameObject slimePurplePrefab;
	public GameObject slimeGreenPrefab;
	public GameObject eyePrefab;
	public GameObject tinyPrefab;
	public GameObject berzerkerPrefab;
	public GameObject flyingSkullPrefab;
	public GameObject tentaclePurplePrefab;

	[Header("Bosses")]
	public GameObject slimeKingPrefab;

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

	[Header("Scrolls")]
	public GameObject attackScrollPrefab;
	public GameObject expScrollPrefab;

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
	private List<GameObject> listOfScrolls = new List<GameObject>();

	// list of enemy prefabs.
	private List<GameObject> listOfEnemies = new List<GameObject>();
    private List<GameObject> listOfSlimes = new List<GameObject>();

	void Awake() { instance = this; }
	public GameObject GetPlayerInstance() { return playerInstance; }
	public List<GameObject> GetEnemyInstances() { return enemyInstances; }

	public void ClearItemLists() {
		listOfArmors.Clear();
		listOfGold.Clear();
		listOfWeapons.Clear();
		listOfPotions.Clear();
		listOfSpells.Clear();
		listOfEnemies.Clear();
		listOfScrolls.Clear();
        listOfSlimes.Clear();
	}

	public void PopulatePrefabLists() {

		// enemies
		listOfEnemies.Add(impPrefab);
		listOfEnemies.Add(slimeBluePrefab);
		listOfEnemies.Add(slimeGreenPrefab);
		listOfEnemies.Add(slimePurplePrefab);
		listOfEnemies.Add(tinyPrefab);
		listOfEnemies.Add(berzerkerPrefab);
		listOfEnemies.Add(eyePrefab);
		listOfEnemies.Add(flyingSkullPrefab);
		listOfEnemies.Add(tentaclePurplePrefab);

        // specific enemy types
        listOfSlimes.Add(slimeBluePrefab);
        listOfSlimes.Add(slimeGreenPrefab);
        listOfSlimes.Add(slimePurplePrefab);

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

		// scrolls
		listOfScrolls.Add(expScrollPrefab);
		listOfScrolls.Add(attackScrollPrefab);

	}

    public GameObject[] GetSlimeEnemies()
    {
        return listOfSlimes.ToArray();
    }

	/// <summary>
	/// Randomizes item.type and returns it.
	/// </summary>
	/// <returns>The loot type.</returns>
	public Item.Type RandomizeItemType() {
		System.Array values = System.Enum.GetValues(typeof(Item.Type));
		return (Item.Type) values.GetValue(Random.Range(0, values.Length));
	}

	/// <summary>
	/// Randomizes the item rarity.
	/// </summary>
	/// <returns>The item rarity.</returns>
	public Item.Rarity RandomizeItemRarity() {
		System.Array values = System.Enum.GetValues(typeof(Item.Rarity));
		return (Item.Rarity) values.GetValue(Random.Range(0, values.Length));
	}

	public void RemovePlayer() {
		Destroy(playerInstance);
	}

	public void RemoveOwnershipAllItems() {
		if(playerInstance == null) return;
		Inventory inv = playerInstance.GetComponent<Inventory>();
		if(inv.currentArmor != null) inv.currentArmor.GetComponent<Item>().owner = null;
		if(inv.currentSpell != null) inv.currentSpell.GetComponent<Item>().owner = null;
		if(inv.currentUseItem != null) inv.currentUseItem.GetComponent<Item>().owner = null;
		if(inv.currentWeapon != null) inv.currentWeapon.GetComponent<Item>().owner = null;
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
			if(tile.GetComponent<Trap>() != null) continue;

			if(tile.myType == Tile.TileType.Floor) {
				possibleTiles.Add(tileGo);
			}
		}
		return possibleTiles[Random.Range(0, possibleTiles.Count)].GetComponent<Tile>().position;
	}

	public GameObject InstantiateItem(GameObject prefab) {

		// Instantiate Item.
		GameObject go = (GameObject) Instantiate(prefab);

		// set owner to null.
		go.GetComponent<Item>().owner = null;

        // set z level
        go.transform.position = new Vector3(0f, 0f, GameMaster.instance.itemZLevel);

		// save startcolor.
		go.GetComponent<Item>().startColor = go.GetComponentInChildren<SpriteRenderer>().color;

		// add to list.
		itemInstances.Add(go);
		return go;
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
			
			// create a list.
			List<GameObject> useItems = null;

			// randomize if it spawns potions or some other use item.
			// currently 50 - 50 chance
			if(Random.Range(0, 100) > 50) {
				useItems = listOfPotions;
			} else {
				useItems = listOfScrolls;
			}

			while(true) {
				prefab = useItems[Random.Range(0, useItems.Count)];
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

			while(true) {
				prefab = listOfSpells[Random.Range(0, listOfSpells.Count)];
				if(prefab.GetComponent<Item>().myRarity == rarity) {
					break;
				}

				if(safetyCounter > listOfSpells.Count + safetyCount) {
					prefab = null;
					break;
				}

				safetyCounter++;
			}

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

	public void InstantiateEnemy(Vector2 pos, GameObject enemyPrefab = null) {

		if(enemyParent == null) enemyParent = new GameObject("EnemyParent");

        GameObject enemyInst = null;

        if (enemyPrefab == null)
        {
            enemyInst = (GameObject)Instantiate(listOfEnemies[Random.Range(0, listOfEnemies.Count)],
                new Vector3(pos.x, pos.y, GameMaster.instance.enemyZLevel), Quaternion.identity);
        }
        else
        {
            enemyInst = (GameObject)Instantiate(enemyPrefab, new Vector3(pos.x, pos.y, GameMaster.instance.enemyZLevel), Quaternion.identity);
        }

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(new Vector2(pos.x, pos.y));
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = enemyInst;

		enemyInst.GetComponent<Enemy>().position = tile.position;

		enemyInst.transform.SetParent(enemyParent.transform);

		enemyInstances.Add(enemyInst);

		GameMaster.instance.enemyCount++;
	}

	public void InstantiateEnemyRandomPos() {
		Vector2 spawnPos = GetFreeInstPosition();
		InstantiateEnemy(spawnPos);
	}

	public void InstantiateEnemies() {
		int count = 0;
		if(GameMaster.instance.randomizeEnemyStartCount) {
			
			// get free floor tile count.
			int maxCount = DungeonGenerator.instance.GetNumberOfTilesOfType(Tile.TileType.Floor);

			// one is player.
			maxCount--;

			// lock the max count.
			if(maxCount > GameMaster.instance.absoluteMaxEnemyCount) maxCount = GameMaster.instance.absoluteMaxEnemyCount;

			// randomize.
			count = Random.Range(10, maxCount);

		} else {
			count = GameMaster.instance.defaultMaxEnemyCountPerDungeon;
		}

		// instantiate.
		for(int i = 0; i < count; i++) {InstantiateEnemyRandomPos(); }
	}

	public GameObject InstantiatePlayer(string pname, bool simpleInstantiate = false) {

		if(simpleInstantiate) {
			playerInstance = (GameObject) Instantiate(playerPrefab);
			return playerInstance;
		}

		Vector2 spawnPos = GetFreeInstPosition();
	
		playerInstance = (GameObject) Instantiate(playerPrefab, new Vector3(spawnPos.x, spawnPos.y, GameMaster.instance.playerZLevel), Quaternion.identity);
		playerInstance.GetComponent<Player>().position = spawnPos;

		playerInstance.name = pname;
		playerInstance.GetComponent<Actor>().actorName = pname;

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(spawnPos);
		Tile tile = tileGo.GetComponent<Tile>();

		tile.actor = playerInstance;

		return playerInstance;
	}
}
