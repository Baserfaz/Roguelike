using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public enum JournalType { System, Combat, Item }

	public static GUIManager instance;

	[Header("GUI elements")]
	public GameObject mainGUI;
	public GameObject topBar;
	public GameObject turnCounter;
	public GameObject journalList;
	public GameObject dungeonName;

	[Header("MainMenu elements")]
	public GameObject mainmenuGUI;

	[Header("Statusbar elements")]
	public GameObject healthStatus;
	public GameObject attackStatus;
	public GameObject armorStatus;
	public GameObject goldStatus;
	public GameObject useItemStatus;

	private int maxJournalEntries = 30;

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
		UpdateElement(dungeonName);
		UpdateElement(healthStatus);
		UpdateElement(attackStatus);
		UpdateElement(armorStatus);
		UpdateElement(goldStatus);
		UpdateElement(useItemStatus);
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

	public void CreateJournalEntry(string txt, JournalType type) {

		GameObject obj = (GameObject) Instantiate(PrefabManager.instance.journalEntry);
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
		case GUIElementScript.Element.DungeonName:
			obj.GetComponentInChildren<Text>().text = "[LVL " + GameMaster.instance.dungeonLevel + "] " + GameMaster.instance.currentDungeonName;
			break;
		case GUIElementScript.Element.Health:
			obj.GetComponentInChildren<Text>().text = "" + PrefabManager.instance.GetPlayerInstance().GetComponent<Health>().currentHealth;
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

		}

	}
}
