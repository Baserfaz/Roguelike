using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Effects {
	public enum PotionType { Heal, Hurt, Attack, Armor, MaxHP };
	public PotionType type;
	public int amount;
}

public class Potion : Item {
	
	[Header("Potion specific settings.")]
	public Effects[] potionEffects;

	public void Drink() {

		foreach(Effects effect in potionEffects) {
			switch(effect.type) {
			case Effects.PotionType.Heal:
				owner.GetComponent<Health>().HealDamage(effect.amount);
				break;
			case Effects.PotionType.Hurt:
				owner.GetComponent<Health>().TakeDamageSimple(effect.amount);
				break;
			case Effects.PotionType.Armor:
				owner.GetComponent<Actor>().defaultArmor += effect.amount;
				GUIManager.instance.CreateJournalEntry("Potion gave " + effect.amount + " armor.", GUIManager.JournalType.Item);
				break;
			case Effects.PotionType.Attack:
				owner.GetComponent<Actor>().defaultDamage += effect.amount;
				GUIManager.instance.CreateJournalEntry("Potion gave " + effect.amount + " attack.", GUIManager.JournalType.Item);
				break;
			case Effects.PotionType.MaxHP:
				owner.GetComponent<Health>().maxHealth += effect.amount;
				GUIManager.instance.CreateJournalEntry("Potion gave " + effect.amount + " MaxHP.", GUIManager.JournalType.Item);
				break;
			}
		}

		owner.GetComponent<Inventory>().currentUseItem = null;

		DestroyItem();
	}
}
