﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIItemInfo : MonoBehaviour {
	public enum ElementType { SpellSlot, ItemSlot, EffectSlot };
	public ElementType myType;
	public GameObject myItem;
	public StatusEffect myEffect;
	public float startAlphaValue;

	void Awake() { startAlphaValue = GetComponent<CanvasGroup>().alpha; }

	public void SetCanvasGroupAlpha(float a) {
		GetComponent<CanvasGroup>().alpha = a;
	}

	public void ResetCanvasGroupAlpha() {
		GetComponent<CanvasGroup>().alpha = startAlphaValue;
	}
}
