using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Item {

	public enum SpellType { FireBall, IceWall, Rejuvenation }

	[Header("Spell specific settings.")]
	public int cooldown = 2;
	public SpellType spellType = SpellType.FireBall;
	public int damageOrHealAmount = 2;
	public int statusDuration = 5;

	[HideInInspector] public int currentCooldown = 0;

	void Start() { ResetCooldown(); }

	public void ResetCooldown() {
		// after casting, the turn ends so cooldown is automatically -> cooldown - 1.
		currentCooldown = cooldown + 1; 
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
			CastFireBall(tile);
			break;
		case SpellType.IceWall:
			CastIceWall(tile);
			break;
		case SpellType.Rejuvenation:
			CastRejuvenation(tile);
			break;
		}
	}

	private void CastRejuvenation(Tile tile) {
		if(tile.actor != null) {

			StatusEffect effect = StatusEffect.CreateEffect(
				StatusEffect.EffectType.Healing,
				damageOrHealAmount, statusDuration);
			
			tile.actor.GetComponent<Actor>().myStatusEffects.Add(effect);
		
			// Create GUi element.
			if(tile.actor.GetComponent<Player>() != null) {
				GUIManager.instance.CreateStatusBarElement(effect);
			}
		}
	}

	private void CastFireBall(Tile tile) {
		if(tile.actor != null) {
			tile.actor.GetComponent<Health>().TakeDamage(damageOrHealAmount, false);
		}
	}

	private void CastIceWall(Tile tile) {

	}


}
