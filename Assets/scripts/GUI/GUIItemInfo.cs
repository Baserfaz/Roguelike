using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIItemInfo : MonoBehaviour {
	public enum ElementType { SpellSlot, ItemSlot, EffectSlot };
	public ElementType myType;
	public GameObject myItem;
	public StatusEffect myEffect;
}
