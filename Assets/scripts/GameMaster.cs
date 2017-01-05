using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;

	public enum MovementMode { Player, Crosshair }

	[HideInInspector] public int turnCount = 0;
	[HideInInspector] public int enemyCount = 0;
	[HideInInspector] public int dungeonLevel = 1;
	[HideInInspector] public string currentDungeonName = "";

	[HideInInspector] public MovementMode movementMode = MovementMode.Player;

	[Header("General settings")]
	public bool spawnEnemies = false;
	public bool wallsBlockLos = true;
	public bool attacksSubtractDefaultArmor = true;

	[Header("Dungeon settings")]
	public int dungeonHeight = 5;
	public int dungeonWidth = 5;
	public int tileZLevel = 1;
	[Range(1, 50)] public int dungeonSpaciousness = 1;

	[Header("Z-levels")]
	public int playerZLevel = -1;
	public int enemyZLevel = -1;
	public int itemZLevel = 0;
	public int worldGuiZLevel = -2;
	public int crosshairZLevel = -2;
	public int vanityitemsZLevel = 0;
	public int lightZLevel = -3;

	[Header("Enemy settings")]
	public int maxEnemyCountPerDungeon = 15;

	[Header("Special Levels")]
	public Texture2D shopLevel;
	public Texture2D mobCabinetlevel01;

	void Awake() { instance = this; }

	private void UpdatePlayerLos() {
		PrefabManager.instance.GetPlayerInstance().GetComponent<LineOfSightManager>().CalculateLoS();
		DungeonGenerator.instance.UpdateTileColorVisibility();
	}

	// PROGRAM START
	void Start () {
		GUIManager.instance.ShowMainmenu();
		GUIManager.instance.HideGUI();
	}

	public void ResetEverything() {
		PrefabManager.instance.ClearItemLists();
		PrefabManager.instance.RemoveEnemies();
		PrefabManager.instance.RemoveItems();
		DungeonGenerator.instance.DestroyDungeon();
		GUIManager.instance.ClearJournal();
		PrefabManager.instance.RemovePlayer();
		DungeonVanityManager.instance.RemoveVanityItems();
	}

	public void StartNewGame() {

		SpriteManager.instance.RandomizeTileSet();

		// create & populate
		DungeonGenerator.instance.Generate(dungeonWidth, dungeonHeight);
		PrefabManager.instance.PopulateItemLists();

		// instantiate actors
		PrefabManager.instance.InstantiateEnemies();
		PrefabManager.instance.InstantiatePlayer();

		// update player line of sight
		UpdatePlayerLos();

		// GUI & other stuff
		CreateNewRandomDungeonName();
		GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
		GUIManager.instance.CreateJournalEntry("Welcome to the dungeon!", GUIManager.JournalType.System);
		GUIManager.instance.UpdateAllElements();
	}

	public void CreateNewRandomDungeonName() {
		// TODO: random name gen.
		currentDungeonName = "Awesome Dungeon";
	}

	public void EndTurn() {

		HandlePlayerTurn();
		HandleEnemyTurns();
		UpdatePlayerLos();

		turnCount++;

		// decrease the cooldown of the current spell.
		if(PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>().currentSpell != null) {
			PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>().currentSpell.GetComponent<Spell>().DecreaseCooldown();
		}

		GUIManager.instance.UpdateAllElements();

		// after all turns, check if the player is dead
		// -> if it is just end the game.
		if(PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().isDead) {
			GameMaster.instance.ResetEverything();
			GUIManager.instance.ShowMainmenu();
			GUIManager.instance.HideGUI();
		}
	}

	public void ExitDungeon() {

		dungeonLevel++;

		// clear stuff
		PrefabManager.instance.RemoveEnemies();
		PrefabManager.instance.RemoveItems();
		DungeonGenerator.instance.DestroyDungeon();
		DungeonVanityManager.instance.RemoveVanityItems();

		// TODO:
		// randomize if next level is special room or random dungeon floor.

		if(dungeonLevel % 3 == 0) {

			// TODO:
			// randomize special rooms

			// change tileset
			SpriteManager.instance.currentTileSet = SpriteManager.TileSet.Shop;

			// create shop
			MapReader.instance.GenerateDungeonFromImage(shopLevel);

			currentDungeonName = "Gerald's awesome shop";

			GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
			GUIManager.instance.CreateJournalEntry("Dungeon level: " + currentDungeonName, GUIManager.JournalType.System);

		} else if(dungeonLevel % 2 == 0) {

			SpriteManager.instance.RandomizeTileSet();

			// create shop
			MapReader.instance.GenerateDungeonFromImage(mobCabinetlevel01);

			currentDungeonName = "Epic loot...";

			GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
			GUIManager.instance.CreateJournalEntry("Dungeon level: " + currentDungeonName, GUIManager.JournalType.System);

		} else {
			
			SpriteManager.instance.RandomizeTileSet();

			// create new stuff
			DungeonGenerator.instance.Generate(dungeonWidth, dungeonHeight);

			// instantiate enemies
			PrefabManager.instance.InstantiateEnemies();

			CreateNewRandomDungeonName();
			GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
			GUIManager.instance.CreateJournalEntry("Dungeon level: " + dungeonLevel, GUIManager.JournalType.System);
		
			// move player to new location
			PrefabManager.instance.MovePlayerToNewStartLocation();
		}

		UpdatePlayerLos();

		// update GUI
		GUIManager.instance.UpdateAllElements();
	}

	private void HandleEnemyTurns() {
		foreach(GameObject enemy in PrefabManager.instance.GetEnemyInstances()) {
			if(enemy.GetComponent<Enemy>().isActive && enemy.GetComponent<Health>().isDead == false) {
				enemy.GetComponent<Enemy>().DecideNextStep();
			}
		}
	}

	private void HandleTileItemInfo(Player player) {
		// get tile info
		GameObject tileGO = DungeonGenerator.instance.GetTileAtPos(player.position);
		Tile tile = tileGO.GetComponent<Tile>();

		// if there is a shop text delete it.
		if(GUIManager.instance.currentActiveShopGo != null) Destroy(GUIManager.instance.currentActiveShopGo);

		// Show item information if 
		// we are standing on top of the 
		// item and it's in shop state.
		if(tile.item != null) {

			Item item = tile.item.GetComponent<Item>();

			if(item.myState == Item.State.Shop) {
				// create new info text
				GUIManager.instance.currentActiveShopGo = GUIManager.instance.CreatePopUpEntry(item.itemName + "\n" + item.itemDescription + "\n" + "Price: " + item.shopPrice,
					player.position, GUIManager.PopUpType.Other, 0f);
			}
		}

	}

	private void HandlePlayerTurn() {
		GameObject playerGo = PrefabManager.instance.GetPlayerInstance();
		Player player = playerGo.GetComponent<Player>();

		if(player.myNextState == Player.NextMoveState.Move) {
			player.Move();
			HandleTileItemInfo(player);

		} else if(player.myNextState == Player.NextMoveState.Attack) {
			player.Attack();
		} else if(player.myNextState == Player.NextMoveState.Pass || player.myNextState == Player.NextMoveState.Stuck) {
			// do nothing.
		}
	}
}
