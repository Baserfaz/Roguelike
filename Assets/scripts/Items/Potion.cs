using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Effects {
	public enum PotionType { Heal, Hurt, Attack, Armor, MaxHP };
	public PotionType type;
	public int minAmount;
	public int maxAmount;
}

public class Potion : Item {
	
	[Header("Potion specific settings.")]
	public Effects[] potionEffects;

	public void Drink() {

		int amount = 0;

		foreach(Effects effect in potionEffects) {
			switch(effect.type) {
			case Effects.PotionType.Heal:
				owner.GetComponent<Health>().HealDamage(Random.Range(effect.minAmount, effect.maxAmount));
				break;
			case Effects.PotionType.Hurt:
				owner.GetComponent<Health>().TakeDamageSimple(Random.Range(effect.minAmount, effect.maxAmount));
				break;
			case Effects.PotionType.Armor:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				owner.GetComponent<Actor>().defaultArmor += amount;
				GUIManager.instance.CreatePopUpEntry("Armor +" + amount, owner.GetComponent<Actor>().position, GUIManager.PopUpType.Other);
				GUIManager.instance.CreateJournalEntry("Potion gave " + amount + " armor.", GUIManager.JournalType.Item);
				break;
			case Effects.PotionType.Attack:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				owner.GetComponent<Actor>().defaultDamage += amount;
				GUIManager.instance.CreatePopUpEntry("Attack +" + amount, owner.GetComponent<Actor>().position, GUIManager.PopUpType.Other);
				GUIManager.instance.CreateJournalEntry("Potion gave " + amount + " attack.", GUIManager.JournalType.Item);
				break;
			case Effects.PotionType.MaxHP:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				owner.GetComponent<Health>().maxHealth += amount;
				GUIManager.instance.CreatePopUpEntry("MaxHP +" + amount, owner.GetComponent<Actor>().position, GUIManager.PopUpType.Other);
				GUIManager.instance.CreateJournalEntry("Potion gave " + amount + " MaxHP.", GUIManager.JournalType.Item);
				break;
			}
		}

		owner.GetComponent<Inventory>().currentUseItem = null;

		DestroyItem();
	}
}
