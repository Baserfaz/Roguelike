using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	/* TYPE:
	 * Armor -> armor
	 * Weapon -> weapon
	 * Gold -> gold
	 * UsableItem -> potion, magical items, etc.
	 * Spell -> spell
	 */

	public enum Type { Armor, Weapon, Gold, UsableItem, Spell }
	public enum Rarity { Normal, Magical, Rare, Legendary }
	public enum State { Shop, Free }

	public string itemName = "";
	public string itemDescription = "";
	public int shopPrice = 10;
	public Rarity myRarity = Rarity.Normal;
	public State myState = State.Free;

	[HideInInspector] public GameObject owner;
	[HideInInspector] public Color startColor;

	protected void DestroyItem() { 
		PrefabManager.instance.RemoveItemFromList(this.gameObject);
		Destroy(this.gameObject); 
	}

	public void HideItem() { 
		GetComponentInChildren<SpriteRenderer>().color = Color.clear; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Hide();
	}
	public void ShowItem() {
		GetComponentInChildren<SpriteRenderer>().color = startColor; 
		if(GetComponent<shadowController>() != null) GetComponent<shadowController>().Show();
	}

}
