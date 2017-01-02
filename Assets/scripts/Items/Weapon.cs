using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

	public enum DamageType { Physical, Fire, Cold, Shock };

	[Header("Weapon specific settings.")]
	public DamageType damageType;
	public int minDamage = 1;
	public int maxDamage = 2;


}
