using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : Item {

	public enum SpellType { FireBall, IceWall, Rejuvenation }
    public enum AOEType { Cross, Box, Line }

	[Header("Spell specific settings.")]
	public int cooldown = 2;
    public bool isLinear = true;
    public SpellType spellType = SpellType.FireBall;
    public bool isAOE = false;
    public AOEType myAOEType;
    [Range(1, 10)] public int aoeSize = 1;
    public int damageOrHealAmount = 2;
	public int statusDuration = 5;

	[HideInInspector] public int currentCooldown = 0;

	public struct DamageInfo {
		public GameObject damageDealer;
		public Tile targetTile;
	}

	//void Start() { ResetCooldown(); }

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
		}
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

	private void CastRejuvenation(DamageInfo info) {
		if(info.targetTile.actor != null) {

			StatusEffect effect = StatusEffect.CreateEffect(
				StatusEffect.EffectType.Healing,
				damageOrHealAmount, statusDuration);
			
			info.targetTile.actor.GetComponent<Actor>().AddStatusEffect(effect);
		}
	}

	private void CastFireBall(DamageInfo info) {

        // Calculate the tiels that are affected by the spell. 
        GameObject[] aoe = CalculateAOE(info);

        if (aoe == null)
        {
            Debug.LogError("ERROR: Area is null!");
            return;
        }

        // apply things to the tiles.
        foreach (GameObject g in aoe)
        {
            if (g == null) continue;
            Tile t = g.GetComponent<Tile>();
            if (t.actor != null)
            {

                // Leave a burning DOT.
                StatusEffect burn = StatusEffect.CreateEffect(StatusEffect.EffectType.Burning, 1, 5);
                t.actor.GetComponent<Actor>().AddStatusEffect(burn);

                // Take direct damage.
                t.actor.GetComponent<Health>().TakeDamage(damageOrHealAmount, false, info.damageDealer);

            }
        }
	}

	private void CastIceWall(DamageInfo info) {
		// TODO
		Debug.LogError("NOT YET IMPLEMENTED!");
	}


}
