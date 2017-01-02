using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;

	[HideInInspector] public int turnCount = 0;
	[HideInInspector] public int enemyCount = 0;
	[HideInInspector] public int dungeonLevel = 1;
	[HideInInspector] public string currentDungeonName = "";

	[Header("General settings")]
	public bool spawnEnemies = false;
	public bool wallsBlockLos = true;
	public bool attacksSubtractDefaultArmor = true;

	[Header("Dungeon settings")]
	public int dungeonHeight = 5;
	public int dungeonWidth = 5;
	public int tileZLevel = 1;
	[Range(1, 50)] public int dungeonSpaciousness = 1;

	[Header("Player settings")]
	public int playerZLevel = -1;

	[Header("Enemy settings")]
	public int enemyZLevel = -1;
	public int maxEnemyCountPerDungeon = 15;

	[Header("Item settings")]
	public int itemZLevel = 0;

	[Header("Special Levels")]
	public Texture2D shopLevel;

	void Awake() { instance = this; }

	private void UpdatePlayerLos() {
		PrefabManager.instance.GetPlayerInstance().GetComponent<LineOfSightManager>().CalculateLoS();
		DungeonGenerator.instance.UpdateTileColorVisibility();
	}

	// GAME STARTS HERE!!!
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
	}

	public void StartNewGame() {
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

		// TODO:
		// randomize if next level is special room or random dungeon floor.

		if(dungeonLevel % 2 == 0) {

			// TODO:
			// randomize special rooms

			// create shop
			MapReader.instance.GenerateDungeonFromImage(shopLevel);

			currentDungeonName = "Gerald's super shop";

			GUIManager.instance.CreateJournalEntry("Dungeon level: " + currentDungeonName, GUIManager.JournalType.System);

		} else {

			// create new stuff
			DungeonGenerator.instance.Generate(dungeonWidth, dungeonHeight);

			// instantiate enemies
			PrefabManager.instance.InstantiateEnemies();

			CreateNewRandomDungeonName();
			GUIManager.instance.CreateJournalEntry("Dungeon level: " + dungeonLevel, GUIManager.JournalType.System);
		}
			
		// move player to new location
		PrefabManager.instance.MovePlayerToNewStartLocation();

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

	private void HandlePlayerTurn() {
		GameObject playerGo = PrefabManager.instance.GetPlayerInstance();
		Player player = playerGo.GetComponent<Player>();

		if(player.myNextState == Player.NextMoveState.Move) {
			player.Move();
		} else if(player.myNextState == Player.NextMoveState.Attack) {
			player.Attack();
		} else if(player.myNextState == Player.NextMoveState.Pass || player.myNextState == Player.NextMoveState.Stuck) {
			// do nothing.
		}
	}
}
