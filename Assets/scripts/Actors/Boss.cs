using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy {

    [Header("Boss settings")]
    public Ability[] myAbilities;

    public void ReduceCooldownAllAbilities()
    {
        // subtract ability cooldown here.
        foreach (Ability myAbility in myAbilities)
        {
            myAbility.currentCooldown--;
            if (myAbility.currentCooldown < 0) myAbility.currentCooldown = 0;
        }

    }

    public void DoAbility(Ability ability, Vector2 target)
    {

        switch (ability.myAbilityType)
        {
            case Ability.Abilities.Summon:

                // summons enemy.
                PrefabManager.instance.InstantiateEnemy(target, ability.spawnObjects[0]);
                break;

            case Ability.Abilities.Mines:

                // TODO:
                // 1. tiles that activate when stepped.

                break;

            case Ability.Abilities.Heal:

                // TODO:
                // 1. instant heals
                // 2. HOT's

                break;

            case Ability.Abilities.Bombs:

                // TODO:
                // 1. tiles that activate after few turns

                break;

            case Ability.Abilities.Buff:

                // TODO:
                // 1. create a buff and apply it to self or other enemies.

                break;

            case Ability.Abilities.Debuff:

                // TODO:
                // 1. debuffs player

                break;

            case Ability.Abilities.DirectDamage:

                // TODO:
                // 1. spells that do direct damage to player.
                // 2. AOE?

                break;

            default:
                Debug.LogError("Can't find current type of ability!");
                break;
        }
    }

      
}
