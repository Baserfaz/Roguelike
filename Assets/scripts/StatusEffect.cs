using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect {
	
	public enum EffectType { Bleeding, Healing, Armor, Attack };
	public EffectType type;
	public int amount;
	public int duration;
	public bool isApplied = false;

	public static StatusEffect CreateEffect(EffectType type, int amount, int duration) {
		StatusEffect effect = new StatusEffect();
		effect.amount = amount;
		effect.duration = duration;
		effect.type = type;
		return effect;
	}
}
