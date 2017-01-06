using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIElementScript : MonoBehaviour {
	public enum Element { NotUpdatable, Health, Attack, Armor, Gold, UseItem, Spell, SpellCooldown }
	public Element myElement;
}
