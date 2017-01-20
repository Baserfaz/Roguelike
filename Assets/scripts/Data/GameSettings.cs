using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings {

	public enum StartItem { AttackScroll, ExpMultScroll, FireballSpell, RejuvenationSpell, IceBlockSpell, None };

	public string playername = "";
	public StartItem myItem = StartItem.None;

	public static GameSettings CreateSettings(string _playername, StartItem _startItem) {

		GameSettings settings = new GameSettings();

		if(_playername == null || _playername == "") settings.playername = "Nameless";
		else settings.playername = _playername;

		settings.myItem = _startItem;

		return settings;
	}

}
