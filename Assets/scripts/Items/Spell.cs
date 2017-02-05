using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Item {

	public enum SpellType { FireBall, IceWall, Rejuvenation, LightningBolt }
    public enum AOEType { Cross, Box, Line }

	[Header("Spell specific settings.")]
	public int cooldown = 2;
    public bool isLinear = true;
    public SpellType spellType = SpellType.FireBall;
    public bool isAOE = false;
    public AOEType myAOEType;
    [Range(1, 10)] public int aoeSize = 1;
    public int directDamage = 2;
    public int statusDamage = 1;
	public int statusDuration = 5;

	[HideInInspector] public int currentCooldown = 0;

	public struct DamageInfo {
		public GameObject damageDealer;
		public Tile targetTile;
	}

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
		DamageInfo di = new DamageInfo();
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
		case SpellType.LightningBolt:
			CastLightningBolt(di);
			break;
		}

		// sound effect
		SoundManager.instance.PlaySound(SoundManager.Sound.CastSpell);

	}

    public GameObject[] CalculateAOE(DamageInfo di)
    {
        List<GameObject> aoeTiles = new List<GameObject>();
        GameObject current = null;
        Actor caster = owner.GetComponent<Actor>();

        switch (myAOEType)
        {
            case AOEType.Box:
                aoeTiles = DungeonGenerator.instance.GetTilesAroundPosition(di.targetTile.position, aoeSize);
                break;

            case AOEType.Cross:

                for (int y = -aoeSize; y < aoeSize + 1; y++)
                {
                    for (int x = -aoeSize; x < aoeSize + 1; x++)
                    {
                        if (x == 0 || y == 0)
                        {
                             current = DungeonGenerator.instance.GetTileAtPos(new Vector2(x, y) + di.targetTile.position);
                             aoeTiles.Add(current);
                        }
                     }
                 }
                break;

            case AOEType.Line:

                Vector2 dif = (caster.position - di.targetTile.position);

                if (dif.x == 0f || dif.y == 0f)
                {
                    if (caster.position.x < di.targetTile.position.x) // right
                    {
                        for (int x = 0; x < aoeSize; x++)
                        {
                            current = DungeonGenerator.instance.GetTileAtPos(di.targetTile.position + new Vector2(x, 0f));
                            aoeTiles.Add(current);
                        }
                    }
                    else if (caster.position.x > di.targetTile.position.x) // left
                    {
                        for (int x = 0; x < aoeSize; x++)
                        {
                            current = DungeonGenerator.instance.GetTileAtPos(di.targetTile.position - new Vector2(x, 0f));
                            aoeTiles.Add(current);
                        }
                    }
                    else if (caster.position.y < di.targetTile.position.y) // up
                    {
                        for (int y = 0; y < aoeSize; y++)
                        {
                            current = DungeonGenerator.instance.GetTileAtPos(di.targetTile.position + new Vector2(0f, y));
                            aoeTiles.Add(current);
                        }
                    }
                    else if (caster.position.y > di.targetTile.position.y) // down
                    {
                        for (int y = 0; y < aoeSize; y++)
                        {
                            current = DungeonGenerator.instance.GetTileAtPos(di.targetTile.position - new Vector2(0f, y));
                            aoeTiles.Add(current);
                        }
                    }
                }
                break;

            default:
                Debug.LogError("No such AOEType!");
                break;
        }
        return aoeTiles.ToArray();
    }

    private StatusEffect DecideStatusEffect(SpellType type) {

        StatusEffect effect = null;

        switch (type) {
                case SpellType.Rejuvenation:
                    effect = StatusEffect.CreateEffect(StatusEffect.EffectType.Healing, statusDamage, statusDuration);
                    break;
                case SpellType.FireBall:
                    effect = StatusEffect.CreateEffect(StatusEffect.EffectType.Burning, statusDamage, statusDuration);
                    break;
				case SpellType.LightningBolt:

					break;
                default:
                    break;
            }
        return effect;
    }

    private void CastAOE(DamageInfo info, SpellType type) {

        // Calculate the tiels that are affected by the spell. 
        GameObject[] aoe = CalculateAOE(info);

        if (aoe == null)
        {
            Debug.LogError("ERROR: Area is null!");
            return;
        }

        // loop through all of the tiles.
        foreach (GameObject g in aoe)
        {
            if (g == null) continue;

            Tile t = g.GetComponent<Tile>();

            if (t.actor != null) {

                DamageInfo di = new DamageInfo();
                di.damageDealer = owner;
                di.targetTile = t;

                // cast it on a tile.
                CastSingleTarget(di, type);
            }
			// create visual effects here
			CreateVisualEffect(type, t);
        }
    }

	private void CreateVisualEffect(SpellType type, Tile t) {

		GameObject visEffec = null;

		switch(type) {
		case SpellType.FireBall:
			visEffec = (GameObject) Instantiate(PrefabManager.instance.flameAnimationPrefab);
			break;
		case SpellType.LightningBolt:
			visEffec = (GameObject) Instantiate(PrefabManager.instance.lightningBoltAnimationPrefab);
			break;
		default:
			break;
		}

		if(visEffec != null) visEffec.transform.position = new Vector3(t.position.x, t.position.y, GameMaster.instance.playerZLevel - 0.1f);
	}

    private void CastSingleTarget(DamageInfo info, SpellType type) {
        if (info.targetTile.actor != null) {

            StatusEffect effect = null;

            // decide which effect.
            effect = DecideStatusEffect(type);

            // apply effect.
			if(effect != null) info.targetTile.actor.GetComponent<Actor>().AddStatusEffect(effect);

            // decide direct damage.
            switch (type) {
            case SpellType.FireBall:
                info.targetTile.actor.GetComponent<Health>().TakeDamage(directDamage,
				false, info.damageDealer);
				break;
			case SpellType.LightningBolt:
				info.targetTile.actor.GetComponent<Health>().TakeDamage(directDamage,
				false, info.damageDealer);
				break;
             default:
                break;
            }
        }

		CreateVisualEffect(type, info.targetTile);
    }

	private void CastRejuvenation(DamageInfo info) {
        if (isAOE) {
            CastAOE(info, SpellType.Rejuvenation);
        } else {
            CastSingleTarget(info, SpellType.Rejuvenation);
        }
	}
		
	private void CastFireBall(DamageInfo info) {

        if (isAOE) {
            CastAOE(info, SpellType.FireBall);
        } else {
            CastSingleTarget(info, SpellType.FireBall);
        }
	}

	private void CastLightningBolt(DamageInfo info) {
		if (isAOE) {
			CastAOE(info, SpellType.LightningBolt);
		} else {
			CastSingleTarget(info, SpellType.LightningBolt);
		}
	}

	private void CastIceWall(DamageInfo info) {
        if (isAOE) {
            Debug.LogError("NOT YET IMPLEMENTED!");
        } else {
            // create script and it to the tile object.
            IceBlock ib = info.targetTile.gameObject.AddComponent<IceBlock>();
            ib.CreateIceBlock(statusDuration, info.targetTile.position, info.targetTile.myType);

            // stun target.
            // set it to be invulnerable.
            if (info.targetTile.actor != null) {
                StatusEffect eff = StatusEffect.CreateEffect(StatusEffect.EffectType.Stun, 0, statusDuration);
                info.targetTile.actor.GetComponent<Actor>().AddStatusEffect(eff);

                eff = StatusEffect.CreateEffect(StatusEffect.EffectType.Invulnerable, 0, statusDuration);
                info.targetTile.actor.GetComponent<Actor>().AddStatusEffect(eff);

            }
        }
	}
}
