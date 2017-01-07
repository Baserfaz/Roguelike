using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experience : MonoBehaviour {

	private List<int> experienceRanges = new List<int>();

	public int currentExp = 0;
	public int currentLevel = 1;

	private int maxLevel = 30;
	private int baseExp = 25;
	private float expMagnifier = 1.25f;

	void Awake() { CalculateExpRanges(); }

	public void AddExp(int amount) {

		int neededExp = GetLevelRequirementExp(currentLevel + 1);

		currentExp += amount;

		// if we leveled up.
		if(currentExp > neededExp) {
			currentExp = 0;
			currentLevel++;
			GUIManager.instance.CreatePopUpEntry("<size=\"31\">Level Up!</size>\nLevel " + currentLevel, GetComponent<Actor>().position, GUIManager.PopUpType.LevelUp);
			GUIManager.instance.CreateJournalEntry("Current level " + currentLevel, GUIManager.JournalType.LevelUp);
		} else {
			GUIManager.instance.CreatePopUpEntry("+" + amount + "exp", GetComponent<Actor>().position, GUIManager.PopUpType.Other);
			GUIManager.instance.CreateJournalEntry("Gained " + amount + " experience.", GUIManager.JournalType.System);
		}

		// update GUI.
		GUIManager.instance.UpdateExpBar(currentExp, neededExp);
	}

	private void CalculateExpRanges() {
		int exp = 0;
		for(int i = 2; i <= maxLevel; i++) {
			exp = Mathf.FloorToInt(baseExp * expMagnifier * i);
			experienceRanges.Add(exp);

			//Debug.Log("lvl " + i + " exp: " + exp);

		}
	}

	/// <summary>
	/// Gets the level requirement exp.
	/// list starts from lvl 2 --> expRanges[0] = lvl 2 exp
	/// </summary>
	/// <returns>The level requirement exp.</returns>
	/// <param name="level">Level.</param>
	public int GetLevelRequirementExp(int level) {
		level -= 2;
		return experienceRanges[level];
	}


}
