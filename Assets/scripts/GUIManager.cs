﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {

	public enum JournalType { System, Combat, Item, LevelUp, Status }
	public enum PopUpType { Damage, Crit, Miss, Other, Heal, Gold, LevelUp }

	public static GUIManager instance;

	[Header("GUI elements")]
	public GameObject mainGUI;
	public GameObject journalList;
	public GameObject statusBar;

	[Header("MainMenu elements")]
	public GameObject mainmenuGUI;

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

	[Header("GUI prefabs")]
	public GameObject journalEntryPrefab;
	public GameObject popUptextPrefab;
	public GameObject OnGuiTextPrefab;
	public GameObject OnGuiBackground_dungeonNamePrefab;
	public GameObject healthBarPrefab;
	public GameObject effectStatusPrefab;
	public GameObject hoverTextPrefab;

	private int maxJournalEntries = 30;

	[HideInInspector] public GameObject currentActiveShopGo = null;

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

	public void HideGUI() { Hide(mainGUI); }
	public void ShowGUI() { Show(mainGUI); }

	public void HideMainmenu() { Hide(mainmenuGUI); }
	public void ShowMainmenu() { Show(mainmenuGUI); }

	public void UpdateExpBar(int currentExp, int maxExp) {
		Slider slider = expBarStatus.GetComponent<Slider>();
		slider.maxValue = maxExp;
		//slider.value = currentExp;

		StartCoroutine(UpdateExpBarGraphics(Mathf.FloorToInt(slider.value), currentExp));
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

	public string ExtractPlayerName(GameObject settings) { 
		string pname = settings.GetComponent<InputField>().text;
		if(pname == "" || pname == null) pname = "Nameless";
		return pname;
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
	public void CreateOnGuiText(string txt, float fadeTime = 5f, bool useBackground = true) {
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
			background.transform.SetParent(mainGUI.transform);
			obj.transform.SetParent(background.transform);
		} else {
			obj.transform.SetParent(mainGUI.transform);
		}

		// scale the object down.
		//background.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

		// fade background.
		if(background != null) background.GetComponent<PopUpText>().StartFade(fadeTime);
		else obj.GetComponent<PopUpText>().StartFade(fadeTime);
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
		GameObject obj = (GameObject) Instantiate(popUptextPrefab);
		obj.GetComponent<Text>().text = txt;

		Vector2 offset = new Vector2(Random.Range(-0.5f, 0.5f), 0.4f);

		obj.transform.position = new Vector3(pos.x + offset.x, pos.y + offset.y, GameMaster.instance.worldGuiZLevel);

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
			obj.GetComponent<Text>().color = new Color32(193, 193, 193, 255); // gray
			break;
		case PopUpType.Heal:
			obj.GetComponent<Text>().color = new Color32(24, 221, 60, 255); // green
			break;
		case PopUpType.Gold:
			obj.GetComponent<Text>().color = new Color32(254, 198, 1, 255); // yellow
			break;
		case PopUpType.LevelUp:
			obj.GetComponent<Text>().color = new Color32(254, 198, 1, 255); // yellow
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
		
		}

	}
}
