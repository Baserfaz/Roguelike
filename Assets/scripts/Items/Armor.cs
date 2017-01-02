using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : Item {

	[Header("Armor specific settings.")]
	public Weapon.DamageType resistType;
	public int maxArmorRating = 1;

	private int currentArmorRating = 1;

	void Awake() { currentArmorRating = maxArmorRating; }

	public int GetArmorRating() { return currentArmorRating; }

	public void SubtractArmor() {
		currentArmorRating --;
		if(currentArmorRating <= 0) {
			owner.GetComponent<Inventory>().currentArmor = null;
			DestroyItem();
		}
	}

	public void AddArmor() {
		currentArmorRating ++;
		if(currentArmorRating > maxArmorRating) currentArmorRating = maxArmorRating;
	}

}
