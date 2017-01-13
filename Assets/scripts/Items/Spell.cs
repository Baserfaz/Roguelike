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

	private struct damageInfo {
		public GameObject damageDealer;
		public Tile targetTile;
	}

	void Start() { ResetCooldown(); }

	public void ResetCooldown() {
		// after casting, the turn ends so cooldown is automatically -> cooldown - 1.
		currentCooldown = cooldown + 1; 
	}

	public void DecreaseCooldown() {
		currentCooldown --;
		if(currentCooldown < 0) currentCooldown = 0;
	}

	public void Cast(Vector2 targetPos, GameObject damageDealer) {

		// reset the spell cooldown.
		ResetCooldown();

		GameObject tileGo = DungeonGenerator.instance.GetTileAtPos(targetPos);
		Tile tile = tileGo.GetComponent<Tile>();

		// create a new struct that has data:
		// 1. who did the damage
		// 2. what is the targeted tile.
		damageInfo di = new damageInfo();
		di.damageDealer = damageDealer;
		di.targetTile = tile;

		// which spell did we cast?
		switch(spellType) {
		case SpellType.FireBall:
			CastFireBall(di);
			break;
		case SpellType.IceWall:
			CastIceWall(di);
			break;
		case SpellType.Rejuvenation:
			CastRejuvenation(di);
			break;
		}
	}

	private void CastRejuvenation(damageInfo info) {
		if(info.targetTile.actor != null) {

			StatusEffect effect = StatusEffect.CreateEffect(
				StatusEffect.EffectType.Healing,
				damageOrHealAmount, statusDuration);
			
			info.targetTile.actor.GetComponent<Actor>().AddStatusEffect(effect);
		}
	}

	private void CastFireBall(damageInfo info) {
		if(info.targetTile.actor != null) {
			info.targetTile.actor.GetComponent<Health>().TakeDamage(damageOrHealAmount, false, info.damageDealer);
		}
	}

	private void CastIceWall(damageInfo info) {
		// TODO
		Debug.LogError("NOT YET IMPLEMENTED!");
	}


}
