using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Item {

	public enum SpellType { FireBall }

	[Header("Spell specific settings.")]
	public int cooldown = 2;
	public SpellType spellType = SpellType.FireBall;
	public int damage = 2;

	[HideInInspector] public int currentCooldown = 0;

	void Start() { ResetCooldown(); }

	public void ResetCooldown() {
		currentCooldown = cooldown + 1; // after casting, the turn ends so cooldown is automatically -> cooldown - 1.
	}

	public void DecreaseCooldown() {
		currentCooldown --;
		if(currentCooldown < 0) currentCooldown = 0;
	}

	public void Cast(Vector2 targetPos) {

		ResetCooldown();

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(targetPos);
		Tile tile = tileGo.GetComponent<Tile>();

		switch(spellType) {
		case SpellType.FireBall:
			if(tile.actor != null) {
				tile.actor.GetComponent<Health>().TakeDamage(damage, false);
			}
			break;
		}

	}

}
