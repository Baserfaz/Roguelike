using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ability  {

    // Abilities are currently enemy spells.
    // should integrate abilities and spells.
    // TODO...

    public enum Abilities { Summon, Heal, Bombs, Mines, Debuff, Buff, DirectDamage }
    public string Abilityname = "";
    public Abilities myAbilityType;
    public GameObject[] spawnObjects;
    public int abilityDamage = 0;
    public int abilityHeal = 0;
    public StatusEffect myStatusEffect;
    public int cooldown = 0;
    [HideInInspector] public int currentCooldown = 0;

}
