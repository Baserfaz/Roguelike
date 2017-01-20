 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public enum JournalType { System, Combat, Item, LevelUp, Status }
	public enum PopUpType { Damage, Crit, Miss, Other, Heal, Gold, LevelUp }

	public static GUIManager instance;

	[Header("Canvases")]
	public GameObject gameGUI;
	public GameObject characterCreationGUI;
	public GameObject deathScreenGUI;
    public GameObject mainmenuGUI;

	[Header("gameGUI elements")]
	public GameObject journalList;
	public GameObject statusBar;
	public GameObject itemInfo;

	[Header("Deathscreen elements")]
	public GameObject playerNameText;
	public GameObject killerNameText;
	public GameObject dungeonLevelText;

	[Header("Player info elements")]
	public GameObject healthStatus;
	public GameObject attackStatus;
	public GameObject armorStatus;
	public GameObject goldStatus;
	public GameObject useItemStatus;
	public GameObject spellStatus;
	public GameObject spellCooldownStatus;
	public GameObject expBarStatus;
	public GameObject expBarText;
    public GameObject playerLevelStatus;

	[Header("TileInfo elements")]
	public GameObject tileInfoText;

	[Header("GUI prefabs")]
	public GameObject journalEntryPrefab;
	public GameObject popUptextPrefab;
	public GameObject OnGuiTextPrefab;
	public GameObject OnGuiBackground_dungeonNamePrefab;
	public GameObject healthBarPrefab;
	public GameObject effectStatusPrefab;
	public GameObject hoverTextPrefab;

	private int maxJournalEntries = 30;

	// These are for showing the tile information.
	// shopGo -> when the player stands on a shop item.
	// tile -> when the mouse cursor is on top of a tile.
	[HideInInspector] public GameObject currentActiveShopGo = null;
	[HideInInspector] public Tile currentActiveTile = null;

	// esc menu
	[HideInInspector] public GameObject pauseTextGo = null;

	private List<GameObject> listOfStatusElements = new List<GameObject>();

	void Awake() { instance = this; }

	private void Hide(GameObject obj) {
		obj.GetComponent<CanvasGroup>().alpha = 0f;
		obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
		obj.GetComponent<CanvasGroup>().interactable = false;
	}

	private void Show(GameObject obj) {
		obj.GetComponent<CanvasGroup>().alpha = 1f;
		obj.GetComponent<CanvasGroup>().blocksRaycasts = true;
		obj.GetComponent<CanvasGroup>().interactable = true;
	}

	public void HideGUI() { Hide(gameGUI); }
	public void ShowGUI() { Show(gameGUI); }

	public void HideCharacterCreation() {
		// enable aberration
		Hide(characterCreationGUI); 
	}

	public void ShowCharacterCreation() { 
		Show(characterCreationGUI); 
	}

    public void HideMainMenu() { Hide(mainmenuGUI); }
    public void ShowMainMenu() { Show(mainmenuGUI); }

	public void HideDeathScreen() { Hide(deathScreenGUI); }
	public void ShowDeathScreen() { Show(deathScreenGUI); }

	public void UpdateTileInfo(string a) {
		tileInfoText.GetComponent<Text>().text = a;
	}

	public void ClearTileInfo() {
		tileInfoText.GetComponent<Text>().text = "";
	}

    public void StartTutorial()
    {
        

    }

    public void ExitGame()
    {
        Application.Quit();
    }

	public void UpdateDeathScreen() {

		Player player = PrefabManager.instance.GetPlayerInstance().GetComponent<Player>();

		// update name.
		playerNameText.GetComponent<Text>().text = 
			player.GetComponent<Actor>().actorName + " (LVL " + player.GetComponent<Experience>().currentLevel + ")";

		// killer name.
		if(player.GetComponent<Health>().lastDmgDealer != null) {
			killerNameText.GetComponent<Text>().text =
				player.GetComponent<Health>().lastDmgDealer.GetComponent<Actor>().actorName + 
				" (HP " + player.GetComponent<Health>().lastDmgDealer.GetComponent<Health>().currentHealth + ")";
		} else {
			killerNameText.GetComponent<Text>().text = "Unknown force";
		}

		// dungeon level.
		dungeonLevelText.GetComponent<Text>().text =
			GameMaster.instance.dungeonLevel + "";
	}

	public void UpdateExpBar(int currentExp, int maxExp) {
		Slider slider = expBarStatus.GetComponent<Slider>();
		slider.maxValue = maxExp;

		if(currentExp == 0) {
			slider.value = currentExp;
		} else {
			StartCoroutine(UpdateExpBarGraphics(Mathf.FloorToInt(slider.value), currentExp));
		}
	}

	private IEnumerator UpdateExpBarGraphics(int currentValue, int targetValue) {

		Slider slider = expBarStatus.GetComponent<Slider>();

		float currentTime = 0f;
		float maxTime = 1f;

		while(currentTime < maxTime) {

			currentTime += Time.deltaTime;

			slider.value = Mathf.Lerp(currentValue, targetValue, currentTime/maxTime);

			yield return null;
		}

		// to make sure value is correct in the end.
		slider.value = targetValue;

	}

	public void UpdateAllElements() {
		UpdateElement(healthStatus);
		UpdateElement(attackStatus);
		UpdateElement(armorStatus);
		UpdateElement(goldStatus);
		UpdateElement(useItemStatus);
		UpdateElement(spellStatus);
		UpdateElement(spellCooldownStatus);
		UpdateElement(expBarText);
        UpdateElement(playerLevelStatus);
	}

	/// <summary>
	/// Updates the status elements.
	/// Run this on every turn.
	/// </summary>
	public void UpdateStatusElements() {
		foreach(GameObject go in listOfStatusElements) {
			int duration = int.Parse(go.GetComponentInChildren<Text>().text);
			duration--;

			if(duration > 0) go.GetComponentInChildren<Text>().text = duration + "";
			else go.GetComponentInChildren<Text>().text = "0";
		}

		// remove old elements.
		for(int i = listOfStatusElements.Count - 1; i >= 0; i--) {
			GameObject g = listOfStatusElements[i];

			if(int.Parse(g.GetComponentInChildren<Text>().text) <= 0) {

				// TODO:
				// 1. journal entry that some specific effect stopped.

				listOfStatusElements.Remove(g);
				Destroy(g);
			}
		}

	}

	public void RemoveAllStatusElements() {
		for(int i = listOfStatusElements.Count - 1; i >= 0; i--) {
			GameObject g = listOfStatusElements[i];
			listOfStatusElements.Remove(g);
			Destroy(g);
		}
	}

	private void DeleteOldJournalEntries() {
		int count = journalList.transform.childCount;
		if(count > maxJournalEntries) {
			int difference = count - maxJournalEntries;
			for(int i = 0; i < difference; i++) {
				Destroy(journalList.transform.GetChild(i).gameObject);
			}
		}
	}

	public void ClearJournal() {
		for(int i = 0; i < journalList.transform.childCount; i++) { 
			Destroy(journalList.transform.GetChild(i).gameObject);
		}
	}

	public void CreateStatusBarElement(StatusEffect effect) {

		GameObject inst = (GameObject) Instantiate(GUIManager.instance.effectStatusPrefab);

		// sprite
		switch(effect.type) {
		case StatusEffect.EffectType.Healing:
			
			inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.GUIStatusHealing);
			break;

		case StatusEffect.EffectType.Bleeding:
			
			inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.GUIStatusBleeding);
			break;

		case StatusEffect.EffectType.Armor:

			if(effect.amount < 0) {
				inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
					SpriteManager.SpriteType.GUIStatusDefDebuff);
			} else {
				inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
					SpriteManager.SpriteType.GUIStatusDefBuff);
			}
			break;

		case StatusEffect.EffectType.Attack:

			if(effect.amount < 0) {
				inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
					SpriteManager.SpriteType.GUIStatusAttDebuff);
			} else {
				inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
					SpriteManager.SpriteType.GUIStatusAttBuff);
			}
			break;

		case StatusEffect.EffectType.ExpMultiplier:

			inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.GUIStatusExpMult);

			break;

		case StatusEffect.EffectType.Stun:

			inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
				SpriteManager.SpriteType.GUIStatusStun);

			break;

        case StatusEffect.EffectType.Burning:

            inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
            SpriteManager.SpriteType.GUIStatusBurning);
            break;

        case StatusEffect.EffectType.Invulnerable:
            inst.GetComponent<Image>().sprite = SpriteManager.instance.CreateTexture(
            SpriteManager.SpriteType.GUIStatusInvulnerable);
            break;

		default:
			Debug.LogError("Can't find sprite.");
			break;

		}

		// duration text
		inst.GetComponentInChildren<Text>().text = effect.duration + "";

		// parent the object.
		inst.transform.SetParent(statusBar.transform);

		// add to a list
		listOfStatusElements.Add(inst);

		// set the reference for hover text to use.
		inst.GetComponent<GUIItemInfo>().myEffect = effect;
	}

	/// <summary>
	/// Instantiates a GUI text element that is bind to the main gui.
	/// </summary>
	/// <param name="txt">Text.</param>
	/// <param name="fadeTime">Fade time.</param>
	public GameObject CreateOnGuiText(string txt, float fadeTime = 5f, bool useBackground = true) {
		GameObject obj = (GameObject) Instantiate(OnGuiTextPrefab);
		GameObject background = null;

		if(useBackground) {
			background = (GameObject) Instantiate(OnGuiBackground_dungeonNamePrefab);
			background.transform.position = Vector3.zero;
			background.transform.SetParent(obj.transform);

			// set the anchors.
			RectTransform rt = background.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0, 0);
			rt.anchorMax = new Vector2(1, 1);
			rt.pivot = new Vector2(0.5f, 0.5f);
		}

		// set text
		obj.GetComponent<Text>().text = txt;

		// position text.
		obj.transform.position = new Vector3(Screen.width/2, Screen.height/2 + Screen.height/3.5f, 0f);

		// change the parents so the text is in front of background.
		if(background != null) {
			background.transform.SetParent(gameGUI.transform);
			obj.transform.SetParent(background.transform);
		} else {
			obj.transform.SetParent(gameGUI.transform);
		}

		// scale the object down.
		if(background != null) background.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

		// fade background.
		if(fadeTime > 0f) {
			if(background != null) background.GetComponent<PopUpText>().StartFade(fadeTime);
			else obj.GetComponent<PopUpText>().StartFade(fadeTime);
		}

		// return the object.
		return obj;
	}

	/// <summary>
	/// Create popup entry.
	/// Returns instance.
	/// If fadeTime <= zero, then popup entry doesn't fade away.
	/// </summary>
	/// <returns>The pop up entry.</returns>
	/// <param name="txt">Text.</param>
	/// <param name="pos">Position.</param>
	/// <param name="type">Type.</param>
	/// <param name="fadeTime">Fade time.</param>
	public GameObject CreatePopUpEntry(string txt, Vector2 pos, PopUpType type, float fadeTime = 1f, TextAnchor alignment = TextAnchor.MiddleCenter) {
		GameObject obj = (GameObject) Instantiate(popUptextPrefab);
		Text txtobj = obj.GetComponent<Text>();

		// set the text.
		txtobj.text = txt;

		// set the alignment.
		txtobj.alignment = alignment;

		// calculate offset.
		Vector2 offset = new Vector2(Random.Range(-0.5f, 0.5f), 0.4f);

		// set the position.
		obj.transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, GameMaster.instance.worldGuiZLevel);

		// set the color.
		switch(type) {
		case PopUpType.Damage:
			txtobj.color = new Color32(155, 28, 17, 255); // red
			break;
		case PopUpType.Crit:
			txtobj.color = new Color32(207, 209, 26, 255); // yellow
			break;
		case PopUpType.Miss:
			txtobj.color = new Color32(194, 225, 194, 255); // gray/green
			break;
		case PopUpType.Other:
			txtobj.color = new Color32(193, 193, 193, 255); // gray
			break;
		case PopUpType.Heal:
			txtobj.color = new Color32(24, 221, 60, 255); // green
			break;
		case PopUpType.Gold:
			txtobj.color = new Color32(254, 198, 1, 255); // yellow
			break;
		case PopUpType.LevelUp:
			txtobj.color = new Color32(254, 198, 1, 255); // yellow
			break;
		}

		// start fade effect if fadetime is greater than zero.
		if(fadeTime > 0f) obj.GetComponent<PopUpText>().StartFadeUp(fadeTime);

		// lastly return the object.
		return obj;
	}

	public void ShowTileInfo(Tile tile) {

		if(currentActiveTile != null) {
			if(currentActiveTile.Equals(tile)) return;
		}

		// set this tile to be active.
		currentActiveTile = tile;

		string tileInformation = "";
		bool isPlayer = false;

		// check if the tile has player on it.
		// -> show item information beneath.
		if(tile.actor != null) {
			if(tile.actor.GetComponent<Player>() != null) {
				isPlayer = true;
			}
		}

		if(tile.actor != null && isPlayer == false) {

			// there is an actor on the tile.
			Enemy e = tile.actor.GetComponent<Enemy>();
			Health h = tile.actor.GetComponent<Health>();

			// TODO:
			// 1. add damage range
			// 2. add damage type (?)
			// 3. add buffs/debuffs

			tileInformation = "<size=21>" + e.actorName + "</size>\n" +
				"<size=16>" + e.enemyDescription + "\nHP " + h.currentHealth + "/" + h.maxHealth + "</size>";
			

		} else if(tile.item != null) {

			// TODO:
			// Compare current vs new item.

			// there is an item on the tile.
			Item item = tile.item.GetComponent<Item>();

			// general information about item.
			tileInformation = "<size=21>" + item.itemName + "</size>\n" +
				"<size=16>" + item.itemDescription + "\n" + 
				"Rarity: " + item.myRarity + "</size>";

			if(item.GetComponent<Weapon>() != null) {

				// item is weapon. 
				Weapon weapon = item.GetComponent<Weapon>();

				tileInformation += "\n<size=16>DMG: " + weapon.minDamage + "-" + weapon.maxDamage + "</size>";
				
			} else if(item.GetComponent<Armor>() != null) {
			
				//item is armor
				Armor armor = item.GetComponent<Armor>();

				tileInformation += "\n<size=16>Armor: " + armor.GetArmorRating() + "</size>";

			} else if(item.GetComponent<Spell>() != null) {

				// item is spell
				Spell spell = item.GetComponent<Spell>();

				tileInformation += "\n<size=16>Type: " + spell.spellType + "\nCooldown: " + spell.cooldown +
					"\nDMG: " + spell.directDamage + "</size>";

			} else if(item.GetComponent<UseItem>() != null) {

				// item is use item
				UseItem ui = item.GetComponent<UseItem>();

				foreach(UseItem.Effects e in ui.useItemEffects) {
					tileInformation += "\n<size=16>Effect: " + e.type + "\nduration: " + e.effectDuration +
						" range: " + e.minAmount + "-" + e.maxAmount + "</size>";
				}
			}

		} else {
			// if there is nothing to show,
			// destroy the text obj and return.
			ClearTileInfo();
			return;
		}

		// destroy the earlier text object.
		ClearTileInfo();

		// update text
		UpdateTileInfo(tileInformation);

	}

	public void CreateJournalEntry(string txt, JournalType type) {

		GameObject obj = (GameObject) Instantiate(journalEntryPrefab);
		obj.transform.SetParent(journalList.transform);

		string totalTxt = "";

		switch(type) {
		case JournalType.System:
			totalTxt = "<color=#FCA311>[System] </color>";
			break;
		case JournalType.Combat:
			totalTxt = "<color=#7D1128>[Combat] </color>";
			break;
		case JournalType.Item:
			totalTxt = "<color=#2E354C>[Item] </color>";
			break;
		case JournalType.LevelUp:
			totalTxt = "<color=#FEC601>[LVLUP] </color>";
			break;
		case JournalType.Status:
			totalTxt = "<color=#E2DDB1>[Status] </color>";
			break;
		}

		obj.GetComponentInChildren<Text>().text = totalTxt + txt;
		DeleteOldJournalEntries();
	}

	private void UpdateElement(GameObject obj) {

		Actor playerActor = PrefabManager.instance.GetPlayerInstance().GetComponent<Actor>();

		switch(obj.GetComponent<GUIElementScript>().myElement) {

		case GUIElementScript.Element.NotUpdatable:
			break;

		case GUIElementScript.Element.Health:
			
			obj.GetComponentInChildren<Text>().text = "" + PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().currentHealth +
				"(" + PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().maxHealth + ")";
			break;

		case GUIElementScript.Element.Armor:

			int totalArmor = playerActor.defaultArmor + playerActor.buffedArmor;

			string armorText = "" + totalArmor;

			if(playerActor.GetComponent<Inventory>().currentArmor != null) {
				armorText = "" + (totalArmor + playerActor.GetComponent<Inventory>().currentArmor.GetComponent<Armor>().GetArmorRating());
			}

			obj.GetComponentInChildren<Text>().text = "" + armorText;
			break;
		
		case GUIElementScript.Element.Attack:

			int totalAtt = playerActor.defaultDamage + playerActor.buffedDamage;

			string attackText = totalAtt + "-" + totalAtt;

			if(playerActor.GetComponent<Inventory>().currentWeapon != null) {
				attackText = (totalAtt + playerActor.GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>().minDamage) + 
					"-" + (totalAtt + playerActor.GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>().maxDamage); 
			}

			obj.GetComponentInChildren<Text>().text = attackText;
			break;

		case GUIElementScript.Element.Gold:
			
			obj.GetComponentInChildren<Text>().text = "" + playerActor.GetComponent<Inventory>().currentGold;
			break;

		case GUIElementScript.Element.UseItem:
			
			if(playerActor.GetComponent<Inventory>().currentUseItem != null) {

				obj.GetComponentInChildren<Image>().sprite = playerActor.GetComponent<Inventory>().currentUseItem.GetComponentInChildren<SpriteRenderer>().sprite;
				obj.GetComponentInChildren<Image>().color = Color.white;

				// set the item reference for hover text to use.
				obj.GetComponentInParent<GUIItemInfo>().myItem = playerActor.GetComponent<Inventory>().currentUseItem;
			} else {
				obj.GetComponentInChildren<Image>().sprite = null;
				obj.GetComponentInChildren<Image>().color = Color.clear;

				// set the item reference for hover text to use.
				obj.GetComponentInParent<GUIItemInfo>().myItem = null;
			}
			break;

		case GUIElementScript.Element.Spell:
			
			if(playerActor.GetComponent<Inventory>().currentSpell != null) {

				GameObject currentSpell = playerActor.GetComponent<Inventory>().currentSpell;

				// set the item reference for hover text to use.
				obj.GetComponentInParent<GUIItemInfo>().myItem = currentSpell;

				obj.GetComponentInChildren<Image>().sprite = currentSpell.GetComponentInChildren<SpriteRenderer>().sprite;

				if(currentSpell.GetComponent<Spell>().currentCooldown > 0) {
					obj.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.5f);
				} else {
					obj.GetComponentInChildren<Image>().color = Color.white;
				}
			} else {
				obj.GetComponentInChildren<Image>().sprite = null;
				obj.GetComponentInChildren<Image>().color = Color.clear;

				// set the item reference for hover text to use.
				obj.GetComponentInParent<GUIItemInfo>().myItem = null;
			}
			break;

		case GUIElementScript.Element.SpellCooldown:
			
			if(playerActor.GetComponent<Inventory>().currentSpell != null) {

				if(playerActor.GetComponent<Inventory>().currentSpell.GetComponent<Spell>().currentCooldown > 0) {
					obj.GetComponentInChildren<Text>().text = "" + playerActor.GetComponent<Inventory>().currentSpell.GetComponent<Spell>().currentCooldown;
				} else {
					obj.GetComponentInChildren<Text>().text = "rdy";
				}
			
			} else {
				obj.GetComponentInChildren<Text>().text = "";
			}
			break;

		case GUIElementScript.Element.ExpText:

			Experience exp = playerActor.GetComponent<Experience>();

			obj.GetComponent<Text>().text = exp.currentExp +
				"/" + exp.GetLevelRequirementExp(exp.currentLevel + 1);

			break;
		
            case GUIElementScript.Element.PlayerLevel:

            obj.GetComponentInChildren<Text>().text = "Level " + playerActor.GetComponent<Experience>().currentLevel;

            break;

		default:
			Debug.LogError("Can't find GUIElementScript.Element!");
			break;

		}
	}
}
