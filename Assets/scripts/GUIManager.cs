using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public enum JournalType { System, Combat, Item }
	public enum PopUpType { Damage, Crit, Miss, Other, Heal }

	public static GUIManager instance;

	[Header("GUI elements")]
	public GameObject mainGUI;
	public GameObject topBar;
	public GameObject turnCounter;
	public GameObject journalList;

	[Header("MainMenu elements")]
	public GameObject mainmenuGUI;

	[Header("Statusbar elements")]
	public GameObject healthStatus;
	public GameObject attackStatus;
	public GameObject armorStatus;
	public GameObject goldStatus;
	public GameObject useItemStatus;
	public GameObject spellStatus;
	public GameObject spellCooldownStatus;

	[Header("GUI prefabs")]
	public GameObject journalEntryPrefab;
	public GameObject popUptextPrefab;
	public GameObject OnGuiTextPrefab;

	private int maxJournalEntries = 30;

	[HideInInspector] public GameObject currentActiveShopGo = null;

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

	public void HideGUI() { Hide(mainGUI); }
	public void ShowGUI() { Show(mainGUI); }

	public void HideMainmenu() { Hide(mainmenuGUI); }
	public void ShowMainmenu() { Show(mainmenuGUI); }

	public void UpdateAllElements() {
		UpdateElement(turnCounter);
		UpdateElement(healthStatus);
		UpdateElement(attackStatus);
		UpdateElement(armorStatus);
		UpdateElement(goldStatus);
		UpdateElement(useItemStatus);
		UpdateElement(spellStatus);
		UpdateElement(spellCooldownStatus);
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

	public void CreateOnGuiText(string txt, float fadeTime = 3f) {
		GameObject obj = (GameObject) Instantiate(OnGuiTextPrefab);
		obj.GetComponent<Text>().text = txt;
		obj.transform.SetParent(mainGUI.transform);
		obj.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0f);
		obj.GetComponent<PopUpText>().StartFade(fadeTime);
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
	public GameObject CreatePopUpEntry(string txt, Vector2 pos, PopUpType type, float fadeTime = 1f) {
		float Yoffset = 0.4f;
		GameObject obj = (GameObject) Instantiate(popUptextPrefab);
		obj.GetComponent<Text>().text = txt;
		obj.transform.position = new Vector3(pos.x, pos.y + Yoffset, GameMaster.instance.worldGuiZLevel);

		switch(type) {
		case PopUpType.Damage:
			obj.GetComponent<Text>().color = new Color32(155, 28, 17, 255); // red
			break;
		case PopUpType.Crit:
			obj.GetComponent<Text>().color = new Color32(207, 209, 26, 255); // yellow
			break;
		case PopUpType.Miss:
			obj.GetComponent<Text>().color = new Color32(194, 225, 194, 255); // gray/green
			break;
		case PopUpType.Other:
			obj.GetComponent<Text>().color = new Color32(193, 180, 174, 255); // gray
			break;
		case PopUpType.Heal:
			obj.GetComponent<Text>().color = new Color32(24, 221, 60, 255); // green
			break;
		}

		if(fadeTime > 0f) obj.GetComponent<PopUpText>().StartFadeUp(fadeTime);

		return obj;
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
		}

		obj.GetComponentInChildren<Text>().text = totalTxt + txt;
		DeleteOldJournalEntries();
	}

	private void UpdateElement(GameObject obj) {

		Actor playerActor = PrefabManager.instance.GetPlayerInstance().GetComponent<Actor>();

		switch(obj.GetComponent<GUIElementScript>().myElement) {
		case GUIElementScript.Element.NotUpdatable:
			break;
		case GUIElementScript.Element.TurnCounter:
			obj.GetComponentInChildren<Text>().text = "Turn: " + GameMaster.instance.turnCount;
			break;
		case GUIElementScript.Element.Health:
			obj.GetComponentInChildren<Text>().text = "" + PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().currentHealth +
				"(" + PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().maxHealth + ")";
			break;
		case GUIElementScript.Element.Armor:

			string armorText = "" + playerActor.defaultArmor;

			if(PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>().currentArmor != null) {
				armorText = "" + (playerActor.defaultArmor + PrefabManager.instance.GetPlayerInstance().GetComponent<Inventory>().currentArmor.GetComponent<Armor>().GetArmorRating());
			}

			obj.GetComponentInChildren<Text>().text = "" + armorText;
			break;
		
		case GUIElementScript.Element.Attack:

			string attackText = playerActor.defaultDamage + "-" + playerActor.defaultDamage;

			if(playerActor.GetComponent<Inventory>().currentWeapon != null) {
				attackText = "" + (playerActor.defaultDamage + playerActor.GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>().minDamage) + 
					"-" + (playerActor.defaultDamage + playerActor.GetComponent<Inventory>().currentWeapon.GetComponent<Weapon>().maxDamage); 
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
			} else {
				obj.GetComponentInChildren<Image>().sprite = null;
				obj.GetComponentInChildren<Image>().color = Color.clear;
			}
			break;
		case GUIElementScript.Element.Spell:
			if(playerActor.GetComponent<Inventory>().currentSpell != null) {

				GameObject currentSpell = playerActor.GetComponent<Inventory>().currentSpell;

				obj.GetComponentInChildren<Image>().sprite = currentSpell.GetComponentInChildren<SpriteRenderer>().sprite;

				if(currentSpell.GetComponent<Spell>().currentCooldown > 0) {
					obj.GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.5f);
				} else {
					obj.GetComponentInChildren<Image>().color = Color.white;
				}

			} else {
				obj.GetComponentInChildren<Image>().sprite = null;
				obj.GetComponentInChildren<Image>().color = Color.clear;
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
		}

	}
}
