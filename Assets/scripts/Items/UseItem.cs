using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Item {

	[System.Serializable]
	public struct Effects {
		public enum EffectType { Heal, Hurt, AttackBuff, ArmorBuff, MaxHP, HoT, DoT, ExpMultiplier, Stun };
		public EffectType type;
		public int minAmount;
		public int maxAmount;
		public int effectDuration;
	}

	[Header("Usable item specific settings.")]
	public Effects[] useItemEffects;

	public void Use() {

		int amount = 0;
		StatusEffect myEffect = null;

		foreach(Effects effect in useItemEffects) {
			switch(effect.type) {

			case Effects.EffectType.Heal:

				owner.GetComponent<Health>().HealDamage(Random.Range(effect.minAmount, effect.maxAmount));

				break;

			case Effects.EffectType.Hurt:

				owner.GetComponent<Health>().TakeDamageSimple(Random.Range(effect.minAmount, effect.maxAmount));

				break;

			case Effects.EffectType.ArmorBuff:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.Armor,
					amount,
					effect.effectDuration);

				//owner.GetComponent<Actor>().AddStatusEffect(myEffect);

				GUIManager.instance.CreatePopUpEntry("+Armor",
					owner.GetComponent<Actor>().position,
					GUIManager.PopUpType.Other);

				break;

			case Effects.EffectType.AttackBuff:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.Attack,
					amount,
					effect.effectDuration);

				//owner.GetComponent<Actor>().myStatusEffects.Add(myEffect);

				GUIManager.instance.CreatePopUpEntry("+Attack",
					owner.GetComponent<Actor>().position,
					GUIManager.PopUpType.Other);

				break;

			case Effects.EffectType.MaxHP:

				amount = Random.Range(effect.minAmount, effect.maxAmount);

				owner.GetComponent<Health>().maxHealth += amount;

				GUIManager.instance.CreatePopUpEntry("MaxHP +" + amount, owner.GetComponent<Actor>().position, GUIManager.PopUpType.Other);
				GUIManager.instance.CreateJournalEntry("Potion gave " + amount + " MaxHP.", GUIManager.JournalType.Item);

				break;

			case Effects.EffectType.HoT:

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.Healing,
					Random.Range(effect.minAmount, effect.maxAmount),
					effect.effectDuration);

				//owner.GetComponent<Actor>().myStatusEffects.Add(myEffect);

				// Update GUI
				GUIManager.instance.CreatePopUpEntry("HoT", owner.GetComponent<Actor>().position, GUIManager.PopUpType.Heal);
				GUIManager.instance.CreateJournalEntry(owner.GetComponent<Actor>().actorName + " started healing.", GUIManager.JournalType.Status);

				break;

			case Effects.EffectType.DoT:

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.Bleeding,
					Random.Range(effect.minAmount, effect.maxAmount),
					effect.effectDuration);

				//owner.GetComponent<Actor>().myStatusEffects.Add(myEffect);

				// Update GUI
				GUIManager.instance.CreatePopUpEntry("DoT", owner.GetComponent<Actor>().position, GUIManager.PopUpType.Heal);
				GUIManager.instance.CreateJournalEntry(owner.GetComponent<Actor>().actorName + " started bleeding.", GUIManager.JournalType.Status);

				break;

			case Effects.EffectType.ExpMultiplier:

				// usees only min amount!

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.ExpMultiplier,
					effect.minAmount,
					effect.effectDuration);

				//owner.GetComponent<Actor>().myStatusEffects.Add(myEffect);

				GUIManager.instance.CreatePopUpEntry("ExpMult",
					owner.GetComponent<Actor>().position,
					GUIManager.PopUpType.Other);

				break;

			case Effects.EffectType.Stun:

				myEffect = StatusEffect.CreateEffect(
					StatusEffect.EffectType.Stun,
					Random.Range(effect.minAmount, effect.maxAmount),
					effect.effectDuration);

				//owner.GetComponent<Actor>().myStatusEffects.Add(myEffect);

				GUIManager.instance.CreatePopUpEntry("Stun",
					owner.GetComponent<Actor>().position,
					GUIManager.PopUpType.Other);

				break;

			default:
				Debug.LogError("Can't find Effects.EffectType!");
				break;
			}
		}

		if(myEffect != null) owner.GetComponent<Actor>().AddStatusEffect(myEffect);

		owner.GetComponent<Inventory>().currentUseItem = null;
		DestroyItem();
		GUIManager.instance.UpdateAllElements();
	}

}
