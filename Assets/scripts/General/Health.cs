using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public int currentHealth = 0;
	public int maxHealth = 100;
	public bool isDead = false;
    public bool invulnerable = false;

	[HideInInspector] public GameObject lastDmgDealer = null;

	void Awake() { UpdateCurrentHealth(); }
		
	public void UpdateCurrentHealth() { currentHealth = maxHealth; }

	/// <summary>
	/// Use this for attacks from other actors.
	/// Uses armor in calculations!
	/// </summary>
	/// <param name="amount">Amount.</param>
	/// <param name="isCrit">If set to <c>true</c> is crit.</param>
	public void TakeDamage(int amount, bool isCrit, GameObject dmgDealer) {

        if (invulnerable)
        {
            GUIManager.instance.CreatePopUpEntry("Invulnerable!",
                GetComponent<Actor>().position, GUIManager.PopUpType.Other);
            return;
        }

		// update what hit us last.
		lastDmgDealer = dmgDealer;

		// save amount to different variable.
		int actualDmg = amount;

        // if it was an enemy that the player hit.
        // enemy should aggro the player.
        if (GetComponent<Enemy>() != null)
        {
            if (lastDmgDealer.GetComponent<Player>() != null)
            {
                GetComponent<Enemy>().targetPosition = lastDmgDealer.GetComponent<Actor>().position;
                GetComponent<Enemy>().isActive = true;
            }
        }

		// Damage is reduced by:
		// 1. current armor rating
		// 2. default armor.
		// 3. buffed armor.
		if(GetComponent<Inventory>() != null) {
			if(GetComponent<Inventory>().currentArmor != null) {
				actualDmg -= GetComponent<Inventory>().currentArmor.GetComponent<Armor>().GetArmorRating();
			}
		}

		actualDmg -= GetComponent<Actor>().defaultArmor + GetComponent<Actor>().buffedArmor;

		// safety because
		// we don't want to heal the player.
		if(actualDmg <= 0) {
			actualDmg = 0;

			GUIManager.instance.CreatePopUpEntry("Armor too thick!",
				GetComponent<Actor>().position, GUIManager.PopUpType.Other);

			GUIManager.instance.CreateJournalEntry(
				GetComponent<Actor>().actorName + "'s armor is too thick!",
				GUIManager.JournalType.Combat);

			return;
		}

		// damage.
		currentHealth -= actualDmg;

		if(isCrit) {
			
			GUIManager.instance.CreatePopUpEntry("-" + actualDmg + "HP!",
				GetComponent<Actor>().position, GUIManager.PopUpType.Crit);
			
			GUIManager.instance.CreateJournalEntry(
				"Critical hit! " + GetComponent<Actor>().actorName + " took " + actualDmg + " damage.",
				GUIManager.JournalType.Combat);
			
		} else {
			
			GUIManager.instance.CreatePopUpEntry("-" + actualDmg + "HP",
				GetComponent<Actor>().position, GUIManager.PopUpType.Damage);
			
			GUIManager.instance.CreateJournalEntry(
				GetComponent<Actor>().actorName + " took " + actualDmg + " damage.",
				GUIManager.JournalType.Combat);
			
		}

		// update healthbar if it exists.
		if(GetComponent<HealthBar>() != null) {
			GetComponent<HealthBar>().UpdateHPBar();
		}

		if(currentHealth <= 0) {
			currentHealth = 0;
			Die();
		}
	}

	/// <summary>
	/// Use this to take damage from potions etc.
	/// </summary>
	/// <param name="amount">Amount.</param>
	public void TakeDamageSimple(int amount) {

        if (invulnerable)
        {
            GUIManager.instance.CreatePopUpEntry("Invulnerable!",
                GetComponent<Actor>().position, GUIManager.PopUpType.Other);
            return;
        }

		currentHealth -= amount;

		GUIManager.instance.CreatePopUpEntry("-" + amount + "HP",
			GetComponent<Actor>().position,
			GUIManager.PopUpType.Damage);
		
		GUIManager.instance.CreateJournalEntry(
			GetComponent<Actor>().actorName + " took " + amount + " damage.",
			GUIManager.JournalType.Combat);

		// update healthbar if it exists.
		if(GetComponent<HealthBar>() != null) {
			GetComponent<HealthBar>().UpdateHPBar();
		}

		if(currentHealth <= 0) {
			currentHealth = 0;
			Die();
		}
	}

	public void HealDamage(int amount) {

		bool healedFully = false;

		currentHealth += amount;
		if(currentHealth > maxHealth) {
			currentHealth = maxHealth;
			healedFully = true;
		}

		// update healthbar if it exists.
		if(GetComponent<HealthBar>() != null) {
			GetComponent<HealthBar>().UpdateHPBar();
		}

		if(healedFully) {
			GUIManager.instance.CreatePopUpEntry("FULL HP",
				GetComponent<Actor>().position,
				GUIManager.PopUpType.Heal);

			GUIManager.instance.CreateJournalEntry(
				GetComponent<Actor>().actorName + " fully healed.",
				GUIManager.JournalType.Combat);
		} else {
			GUIManager.instance.CreatePopUpEntry("+" + amount + "HP",
				GetComponent<Actor>().position,
				GUIManager.PopUpType.Heal);

			GUIManager.instance.CreateJournalEntry(
				GetComponent<Actor>().actorName + " healed " + amount + " health.",
				GUIManager.JournalType.Combat);
		}

	}

	private void Die() {

		isDead = true;

		// update tile's actor = null;
		DungeonGenerator.instance.UpdateTileActor(GetComponent<Actor>().position, null);

		// hide healthbar if it exists.
		if(GetComponent<HealthBar>() != null) {
			GetComponent<HealthBar>().Hide();
		}

		// if its enemy.. drop loot.
		if(GetComponent<Enemy>() != null) {

			// player gains experience.
			PrefabManager.instance.GetPlayerInstance().GetComponent<Experience>().AddExp(GetComponent<Actor>().expAmount);

            // check if the enemy can even drop items.
            if (GetComponent<Enemy>().canDropItems)
            {
                // get target tile.
                GameObject targetTile = DungeonGenerator.instance.GetTileAtPos(GetComponent<Actor>().position);

                // drop item under the 
                if (targetTile.GetComponent<Tile>().item == null)
                {

                    ItemDropController.instance.DropItem(targetTile.GetComponent<Tile>().position);

                }
                else
                {

                    // if tile has an item already.
                    // -> calculate new position.
                    targetTile = DungeonGenerator.instance.GetFirstFreeTileNearPosition(GetComponent<Actor>().position);
                    if (targetTile != null)
                    {
                        ItemDropController.instance.DropItem(targetTile.GetComponent<Tile>().position);
                    }
                }
            }

            // deactivate enemy script.
			GetComponent<Enemy>().isActive = false;

			// deactivate actor
			gameObject.SetActive(false);

            // create GUI element.
			GUIManager.instance.CreateJournalEntry(GetComponent<Actor>().actorName + " died.", GUIManager.JournalType.Combat);
		} 
	}
}
