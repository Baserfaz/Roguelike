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

	public void OnLevelUp() {

		// TODO:
		// if we have a certain perk, we could get a buff for example..
		// ----> create perk system??
		// but for now, just give the buff :)

		// TODO:
		// perhaps create a "LEVELUP" statusEffect type 
		// and in GameMaster.handleStatusEffects()
		// we could apply multiple status effects.
		// ---> we could have a single status effect with 
		// ---> a special sprite.

		// now we create two different effects
		// with lame sprites.
		StatusEffect atteff = new StatusEffect();
		atteff.type = StatusEffect.EffectType.Attack;
		atteff.duration = 10;
		atteff.amount = 1;

		StatusEffect defeff = new StatusEffect();
		defeff.type = StatusEffect.EffectType.Armor;
		defeff.duration = 10;
		defeff.amount = 1;

		GetComponent<Actor>().AddStatusEffect(atteff);
		GetComponent<Actor>().AddStatusEffect(defeff);

		// also increase the player's health by an amount.
		Health h = GetComponent<Health>();
		h.maxHealth ++;

		// heal the player to full.
		h.HealDamage(h.maxHealth + 1);


		// sound effect
		SoundManager.instance.PlaySound(SoundManager.Sound.LvlUp);

	}

	public void AddExp(int amount) {

		int neededExp = GetLevelRequirementExp(currentLevel + 1);

		currentExp += Mathf.FloorToInt(amount * GetComponent<Actor>().buffedExpMultiplier);

		// if we leveled up.
		if(currentExp > neededExp) {
			currentExp = 0;
			currentLevel++;

			OnLevelUp();

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
