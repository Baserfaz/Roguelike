using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour {
	public GameObject playerNameGo;
	public GameObject dropdownMenuGo;

	void Start () {
		// add click listener.
		GetComponent<Button>().onClick.AddListener(() => CreateGameSettings());
	}

	// Create a custom game settings object
	// that has all setting information 
	// and pass that to GameMaster.StartGame().
	private void CreateGameSettings() {

		// get the player name
		string playername = playerNameGo.GetComponent<InputField>().text;

		// get the item 
		GameSettings.StartItem startitem = GameSettings.StartItem.None;

		switch(dropdownMenuGo.GetComponent<Dropdown>().value) {
		case 0:
			startitem = GameSettings.StartItem.AttackScroll;
			break;
		case 1:
			startitem = GameSettings.StartItem.ExpMultScroll;
			break;
		case 2:
			startitem = GameSettings.StartItem.FireballSpell;
			break;
        case 3:
            startitem = GameSettings.StartItem.RejuvenationSpell;
            break;
        case 4:
            startitem = GameSettings.StartItem.IceBlockSpell;
            break;

		default:
			startitem = GameSettings.StartItem.None;
			break;
		}

		GameSettings settings = GameSettings.CreateSettings(playername, startitem);
		StartGame(settings);
	}

	private void StartGame(GameSettings settings) {
		GUIManager.instance.HideCharacterCreation();
		GUIManager.instance.ShowGUI();
		GameMaster.instance.StartNewGame(settings);
	}
}
