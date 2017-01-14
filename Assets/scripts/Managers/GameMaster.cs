﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;

	public enum MovementMode { Player, Crosshair }
	public enum GameState { Running, Paused, InMainMenu }

	[HideInInspector] public int turnCount = 0;
	[HideInInspector] public int enemyCount = 0;
	[HideInInspector] public int dungeonLevel = 1;
	[HideInInspector] public string currentDungeonName = "";
	[HideInInspector] public MovementMode movementMode = MovementMode.Player;
	[HideInInspector] public GameState gamestate = GameState.Paused;

	[Header("Controls")]
	public bool allowKeyboardInput = false;
	public bool allowMouseInput = true;

	[Header("Pathfinding settings")]
	public bool allowPathfinding = true;
	public bool useAStar = true;
	public bool allowPathfindInvisibleTiles = true;

	[Header("Movement settings")]
	public bool allowSmoothMovement = true;

	[Header("Debugging")]
	public bool enableEnemyAI = true;
	public bool debugMode = false;

	[Header("General settings")]
	public bool spawnEnemies = false;
	public bool wallsBlockLos = true;
	public bool pickupGoldAutomatically = true;
	public bool attacksSubtractDefaultArmor = true;
	public bool useBlankmainMenuScreen = false;
	public bool randomizeDungeonSize = true;
	public bool randomizeEnemyStartCount = true;

	[Header("Randomized dungeon sizes")]
	[Tooltip("OVERRIDES DUNGEON SETTINGS!")]
	[Range(1, 50)] public int maxWidth = 25;
	[Tooltip("OVERRIDES DUNGEON SETTINGS!")]
	[Range(1, 50)] public int minWidth = 10;
	[Tooltip("OVERRIDES DUNGEON SETTINGS!")]
	[Range(1, 50)] public int maxHeight = 25;
	[Tooltip("OVERRIDES DUNGEON SETTINGS!")]
	[Range(1, 50)] public int minHeight = 10;

	[Header("Dungeon settings")]
	public int dungeonHeight = 5;
	public int dungeonWidth = 5;
	public int tileZLevel = 1;
	[Range(1, 50)] public int dungeonSpaciousness = 1;

	[Header("Door settings")]
	[Range(1, 100)] public int doorSpawnChance = 10;

	[Header("Trap settings")]
	public bool GenerateTraps = true;
	[Range(0, 100)] public int trapSpawnChance = 10;
	public int trapInitialDamage = 1;
	public int trapDoTDamage = 1;
	public int trapDoTDuration = 2;
	public int trapDelayInTurns = 1;
	public bool trapsCauseBleedEffect = true;

	[Header("Special level chance to spawn")]
	[Range(1, 100)] public int ShopChance = 10;
	[Range(1, 100)] public int mobCabinet01Chance = 15;

	[Header("Z-levels")]
	public int playerZLevel = -1;
	public int enemyZLevel = -1;
	public int itemZLevel = 0;
	public int worldGuiZLevel = -2;
	public int crosshairZLevel = -2;
	public int vanityitemsZLevel = 0;
	public int lightZLevel = -3;

	[Header("Enemy settings")]
	public int defaultMaxEnemyCountPerDungeon = 15;
	public int absoluteMaxEnemyCount = 30;

	[Header("Special Levels")]
	public Texture2D shopLevel;
	public Texture2D mobCabinetlevel01;
	public Texture2D pathfindingTestLevel;

	private bool wasLastLevelSpecial = false;
	private bool debugModeRealValue = false;

	void Awake() { 
		instance = this;
		debugModeRealValue = debugMode;
	}

	private void UpdatePlayerLos() {
		PrefabManager.instance.GetPlayerInstance().GetComponent<LineOfSightManager>().CalculateLoS();
		DungeonGenerator.instance.UpdateTileColorVisibility();
	}

	// PROGRAM START
	void Start () {

		gamestate = GameState.InMainMenu;

		// hide GUIs
		GUIManager.instance.HideGUI();
		GUIManager.instance.HideDeathScreen();
		GUIManager.instance.ShowMainmenu();

		// disable aberration
		Camera.main.GetComponent<CameraEffects>().DisableAberration();

		// create mainmenu scene.
		CreateMainMenuScene();

		// CONTINUES -> PLAY BUTTON OnClick
		// --> StartNewGame()
	}

	public void OpenPathfindingTest() {
		ResetEverything();

		debugMode = true;

		// game loop is running.
		gamestate = GameState.Running;

		// randomize tile set.
		SpriteManager.instance.RandomizeTileSet();

		// populate item lists.
		PrefabManager.instance.PopulatePrefabLists();

		// create level
		MapReader.instance.GenerateDungeonFromImage(pathfindingTestLevel);

		// update player line of sight
		UpdatePlayerLos();

		// update GUI
		GUIManager.instance.UpdateAllElements();

	}

	public void CreateMainMenuScene() {
		if(useBlankmainMenuScreen) return;
		GameMaster.instance.debugMode = true;
		SpriteManager.instance.RandomizeTileSet();
		PrefabManager.instance.PopulatePrefabLists();
		StartDungeonCreationProcess();
	}

	public void ResetEverything() {

		if(debugModeRealValue) {
			debugMode = true;
		} else {
			debugMode = false;
		}

		// Reset camera.
		CameraManager cm = Camera.main.GetComponent<CameraManager>();

		// rotation.
		cm.ResetRotation();

		// orthosize.
		Camera.main.orthographicSize = (float) cm.maxZoom / cm.minZoom;

		// reset dungeon level
		dungeonLevel = 1;

		// reset player level
		GameObject player = PrefabManager.instance.GetPlayerInstance();
		if(player != null) player.GetComponent<Experience>().currentLevel = 1;

		// reset status elements GUI
		GUIManager.instance.RemoveAllStatusElements();

		// reset tileinfo text 
		GUIManager.instance.ClearTileInfo();

		PrefabManager.instance.ClearItemLists();
		PrefabManager.instance.RemoveEnemies();

		// set the item owner to be null, that it can be deleted.
		PrefabManager.instance.RemoveOwnershipAllItems();

		// Destroy everything else.
		PrefabManager.instance.RemoveItems();
		DungeonGenerator.instance.DestroyDungeon();
		GUIManager.instance.ClearJournal();
		PrefabManager.instance.RemovePlayer();
		DungeonVanityManager.instance.RemoveVanityItems();
	}

	/// <summary>
	/// Calculates random/set dungeon widths and generates dungeon.
	/// </summary>
	private void StartDungeonCreationProcess() {
		// overrides set dungeonheight and dungeonwidth.
		if(randomizeDungeonSize) {
			dungeonWidth = Random.Range(minWidth, maxWidth);
			dungeonHeight = Random.Range(minHeight, maxHeight);
		} 
		DungeonGenerator.instance.Generate(dungeonWidth, dungeonHeight);
	}

	private void GivePlayerStartItems(Player player, GameSettings.StartItem myItem) {
		GameObject go = null;
		switch(myItem) {
		case GameSettings.StartItem.ExpMultScroll:
			go = PrefabManager.instance.InstantiateItem(PrefabManager.instance.expScrollPrefab);
			break;
		case GameSettings.StartItem.FireballSpell:
			go = PrefabManager.instance.InstantiateItem(PrefabManager.instance.fireballSpellPrefab);
			break;

		case GameSettings.StartItem.AttackScroll:
			go = PrefabManager.instance.InstantiateItem(PrefabManager.instance.attackScrollPrefab);
			break;

		default:
			break;
		}

		if(go != null) {
			player.PickUpItem(go);
		}
	}

	// From play button.
	public void StartNewGame(GameSettings settings) {

		// before anything just reset all.
		ResetEverything();

		// game loop is running.
		gamestate = GameState.Running;

		// randomize tile set.
		SpriteManager.instance.RandomizeTileSet();

		// populate item lists.
		PrefabManager.instance.PopulatePrefabLists();

		// create dungeon
		StartDungeonCreationProcess();

		// instantiate actors
		PrefabManager.instance.InstantiateEnemies();

		// instantiate player.
		GameObject playerinstance = PrefabManager.instance.InstantiatePlayer(settings.playername, false);

		// instantiate item that player chose in main menu.
		GivePlayerStartItems(playerinstance.GetComponent<Player>(), settings.myItem);

		// update player line of sight
		UpdatePlayerLos();

		// update exp bar
		GUIManager.instance.UpdateExpBar(
			PrefabManager.instance.GetPlayerInstance().GetComponent<Experience>().currentExp,
			PrefabManager.instance.GetPlayerInstance().GetComponent<Experience>().GetLevelRequirementExp(2));

		// GUI & other stuff
		CreateNewRandomDungeonName();
		GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
		GUIManager.instance.CreateJournalEntry("Welcome to the dungeon, " + settings.playername + ".", GUIManager.JournalType.System);
		GUIManager.instance.UpdateAllElements();

		// effect
		Camera.main.GetComponent<CameraEffects>().StartBrightUp();
	}

	public void CreateNewRandomDungeonName() {

		// TODO:
		// 1. positive, neutral, evil names
		// 2. decide name from used tileset.

		string[] prefixes = { "Dark", "Damp", "Moldy", "Old", "Hellish", "Agonizing", "Screaming", "Echoing", "Silencing" };
		string [] places = { "Cellar", "Lair", "Sewer", "Catacombs" };
		//string[] suffixes = { "" };

		currentDungeonName = prefixes[Random.Range(0, prefixes.Length)] + " " + places[Random.Range(0, places.Length)];
	}

	public void EndTurn() {
		
		HandlePlayerTurn();
		if(enableEnemyAI) HandleEnemyTurns();
		HandleTraps();
		UpdatePlayerLos();

		turnCount++;

		// update status elements GUI
		GUIManager.instance.UpdateStatusElements();

		// decrease the cooldown of the current spell.
		if(PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>().currentSpell != null) {
			PrefabManager.instance.GetPlayerInstance().
			GetComponent<Inventory>().
			currentSpell.GetComponent<Spell>().
			DecreaseCooldown();
		}

		// update player info
		GUIManager.instance.UpdateAllElements();

		// Check if player is dead.
		// -> run this sequence.
		HandlePlayerDeath();
	}

	private void HandlePlayerDeath() {

		if(PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().isDead) {

			// update gui
			GUIManager.instance.UpdateDeathScreen();

			// show/hide guis
			GUIManager.instance.HideGUI();
			GUIManager.instance.ShowDeathScreen();

			// reset game
			ResetEverything();

			// set the game loop to main menu
			gamestate = GameState.InMainMenu;
		}
	}

	private bool TryGetSpecialLevel() {
		if(Random.Range(0, 100) > 100 - ShopChance) {

			wasLastLevelSpecial = true;

			// change tileset to use shop tiles.
			SpriteManager.instance.currentTileSet = SpriteManager.TileSet.Shop;

			// create shop
			MapReader.instance.GenerateDungeonFromImage(shopLevel);

			// name
			currentDungeonName = "Shopaholic's dream";

			return true;

		} else if(Random.Range(0, 100) > 100 - mobCabinet01Chance) {

			wasLastLevelSpecial = true;

			// randomize tileset
			SpriteManager.instance.RandomizeTileSet();

			// create shop
			MapReader.instance.GenerateDungeonFromImage(mobCabinetlevel01);

			currentDungeonName = "It's not a trap.";

			return true;
		}

		return false;
	}

	private void GetRandomGenLevel() {
		wasLastLevelSpecial = false;

		// randomize tileset once again.
		SpriteManager.instance.RandomizeTileSet();

		// create new randomized dungeon.
		StartDungeonCreationProcess();

		// instantiate enemies.
		PrefabManager.instance.InstantiateEnemies();

		// name it.
		CreateNewRandomDungeonName();

		// move player to new location
		PrefabManager.instance.MovePlayerToNewStartLocation();
	}

	public void ExitDungeon() {

		// camera effect.
		Camera.main.GetComponent<CameraEffects>().StartBrightUp();

		// stop all coroutines!
		PrefabManager.instance.GetPlayerInstance().GetComponent<Player>().StopCoroutines();

		// increement level
		dungeonLevel++;

		// clear stuff
		PrefabManager.instance.RemoveEnemies();
		PrefabManager.instance.RemoveItems();
		DungeonGenerator.instance.DestroyDungeon();
		DungeonVanityManager.instance.RemoveVanityItems();

		// if last level was a special level
		// -> create random level.
		// --> else try to generate special level first.
		if(wasLastLevelSpecial) {
			GetRandomGenLevel();
		} else {
			bool success = TryGetSpecialLevel();
			if(success == false) {
				GetRandomGenLevel();
			} 
		}

		GUIManager.instance.CreateOnGuiText("[LVL " + dungeonLevel + "]\n" + currentDungeonName);
		GUIManager.instance.CreateJournalEntry("Dungeon level: " + currentDungeonName, GUIManager.JournalType.System);

		UpdatePlayerLos();

		// update GUI
		GUIManager.instance.UpdateAllElements();
	}

	private void HandleTraps() {
		foreach(GameObject tile in DungeonGenerator.instance.GetTiles()) {
			if(tile.GetComponent<Trap>() != null) {
				Trap trap = tile.GetComponent<Trap>();

				// if the trap is activated on this turn.
				if(trap.GetActiveStatus() == Trap.State.Active) {

					if(trap.activatedOnTurn + trapDelayInTurns == turnCount) {

						// open spikes
						trap.UpdateGraphics();

						// on next turn this trap will be inactive.
						trap.Deactivate();

						Tile t = tile.GetComponent<Tile>();

						// damage whoever is on the tile.
						if(t.actor != null) {

							// set bleed effect.
							if(trapsCauseBleedEffect) {
								
								// create effect
								StatusEffect bleed = StatusEffect.CreateEffect(
									StatusEffect.EffectType.Bleeding, trapDoTDamage, trapDoTDuration);

								// add effect to the actor.
								t.actor.GetComponent<Actor>().AddStatusEffect(bleed);

								// gui stuff
								GUIManager.instance.CreatePopUpEntry("BLEEDING", t.position, GUIManager.PopUpType.Damage);
								GUIManager.instance.CreateJournalEntry(
									t.actor.GetComponent<Actor>().actorName + " started to bleed.",
									GUIManager.JournalType.Status);
							}

							// initial damage
							t.actor.GetComponent<Health>().TakeDamageSimple(trapInitialDamage);

						}

						// turn the tile to a wall
						// -> actors can't step on spikes after their have popped out.
						t.myType = Tile.TileType.Wall;

					}

				} else if(trap.GetActiveStatus() == Trap.State.Inactive) {
					// close the spikes
					trap.UpdateGraphics();

					// turn the tile to a floor again.
					tile.GetComponent<Tile>().myType = Tile.TileType.Floor;
				}
			}
		}
	}

	private void HandleEnemyTurns() {
		foreach(GameObject enemy in PrefabManager.instance.GetEnemyInstances()) {
			Enemy e = enemy.GetComponent<Enemy>();
			if(e.isActive) {

				// this can cause bleed effect,
				// and there fore it can kill the enemy.
				HandleStatusEffects(e);

				// so we need to check is dead here.
				if(enemy.GetComponent<Health>().isDead == false || enemy.GetComponent<Actor>().myNextState != Actor.NextMoveState.Stunned) {
					e.DecideNextStep();
				}
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

	private void HandleStatusEffects(Actor actor) {

		// Apply each status effect on actor.
		foreach(StatusEffect e in actor.GetStatusEffects()) {

			if(e.duration <= 0) {
				actor.RemoveStatusEffect(e);
				continue;
			}

			switch(e.type) {
			case StatusEffect.EffectType.Bleeding:
				
				actor.GetComponent<Health>().TakeDamageSimple(e.amount);
				break;

			case StatusEffect.EffectType.Healing:
				
				actor.GetComponent<Health>().HealDamage(e.amount);
				break;

			case StatusEffect.EffectType.Armor:

				// can apply effect only once.
				if(e.isApplied == false) {
					e.isApplied = true;
					actor.GetComponent<Actor>().buffedArmor += e.amount;
				}

				break;

			case StatusEffect.EffectType.Attack:

				// can apply effect only once.
				if(e.isApplied == false) {
					e.isApplied = true;
					actor.GetComponent<Actor>().buffedDamage += e.amount;
				}

				break;

			case StatusEffect.EffectType.ExpMultiplier:

				// can apply effect only once.
				if(e.isApplied == false) {
					e.isApplied = true;
					actor.GetComponent<Actor>().buffedExpMultiplier += e.amount / 10f;
				}

				break;

			case StatusEffect.EffectType.Stun:
				
				actor.GetComponent<Actor>().myNextState = Actor.NextMoveState.Stunned;

				break;

			default:
				Debug.LogError("Can't find such status effect at GameMaster.HandleStatusEffects().");
				break;
			}

			e.duration --;
		}

		// remove old status effects
		StatusEffect[] allEffects = actor.GetStatusEffects();

		for(int i = allEffects.Length - 1; i >= 0; i--) {
			StatusEffect e = allEffects[i];
			if(e.duration <= 0) {
				actor.RemoveStatusEffect(e);

				// remove effects.
				if(actor.GetComponent<Player>() != null) {
					
					if(e.type == StatusEffect.EffectType.ExpMultiplier) {
						actor.GetComponent<Player>().buffedExpMultiplier -= e.amount / 10f;
					} else if(e.type == StatusEffect.EffectType.Armor) {
						actor.GetComponent<Actor>().buffedArmor -= e.amount;
					} else if(e.type == StatusEffect.EffectType.Attack) {
						actor.GetComponent<Actor>().buffedDamage -= e.amount;
					}

				}
			}
		}
	}

	private void HandlePlayerTurn() {
		GameObject playerGo = PrefabManager.instance.GetPlayerInstance();
		Player player = playerGo.GetComponent<Player>();

		// handle status effects.
		HandleStatusEffects(player);

		if(player.myNextState == Actor.NextMoveState.Stunned) return;

		// move states.
		if(player.myNextState == Player.NextMoveState.Move) {
			player.Move();
			HandleTileItemInfo(player);
		} else if(player.myNextState == Player.NextMoveState.Attack) {
			player.Attack();
		} else if(player.myNextState == Player.NextMoveState.Pass) {
			// do nothing.
		}
	}
}
